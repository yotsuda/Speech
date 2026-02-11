using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using NAudio.Wave;
using Speech.Core;

namespace Speech.Windows
{
    [Cmdlet(VerbsData.Out, "WindowsSpeech")]
    public class OutWindowsVoiceCmdlet : WindowsCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string? Text { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(VoiceCompleter))]
        public string? Voice { get; set; }

        [Parameter]
        [ValidateRange(0.5, 2.0)]
        public double? Rate { get; set; }

        [Parameter]
        [ValidateRange(0, 100)]
        public int? Volume { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(OutputDeviceCompleter))]
        public string? OutputDevice { get; set; }

        internal class VoiceCompleter : IArgumentCompleter
        {
            public IEnumerable<CompletionResult> CompleteArgument(
                string commandName,
                string parameterName,
                string wordToComplete,
                CommandAst commandAst,
                IDictionary fakeBoundParameters)
            {
                var results = new List<CompletionResult>();

                try
                {
                    var synthesizer = WindowsAudioManager.GetSynthesizer();
                    var voices = synthesizer?.GetInstalledVoices();

                    if (voices != null)
                    {
                        foreach (var voice in voices.Where(v => v.Enabled))
                        {
                            var voiceName = voice.VoiceInfo.Name;

                            if (string.IsNullOrEmpty(wordToComplete) ||
                                voiceName.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                            {
                                var tooltip = $"{voiceName}  Culture:{voice.VoiceInfo.Culture.Name}  Gender:{voice.VoiceInfo.Gender}  Age:{voice.VoiceInfo.Age}";
                                results.Add(new CompletionResult(
                                    $"'{voiceName}'",
                                    voiceName,
                                    CompletionResultType.ParameterValue,
                                    tooltip));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"VoiceCompleter error: {ex.Message}");
                }

                return results;
            }
        }

        private int ConvertToWindowsRate(double rate)
        {
            // 0.5 → -10, 1.0 → 0, 2.0 → 10
            return (int)Math.Round((rate - 1.0) * 20);
        }

        protected override void ProcessRecord()
        {
            try
            {
                var synthesizer = WindowsAudioManager.GetSynthesizer();

                // Get values from parameters or config (ConfigManager handles everything)
                var voice = ConfigManager.GetWindowsVoice(Voice);
                var rate = ConfigManager.GetRate(Rate);
                var volume = ConfigManager.GetVolume(Volume);

                // Apply voice
                if (!string.IsNullOrEmpty(voice))
                {
                    try
                    {
                        synthesizer.SelectVoice(voice);
                    }
                    catch (ArgumentException)
                    {
                        WriteWarning($"Voice '{voice}' not found. Using default voice.");
                    }
                }

                // Apply rate and volume
                synthesizer.Rate = ConvertToWindowsRate(rate);
                synthesizer.Volume = volume;

                // Resolve output device
                var outputDeviceName = ConfigManager.GetOutputDevice(OutputDevice);
                var deviceNumber = OutputDeviceCompleter.FindOutputDeviceIndex(outputDeviceName) ?? -1;

                if (deviceNumber >= 0)
                {
                    // Device specified: synthesize to WAV stream, then play via NAudio on specific device
                    using var wavStream = new MemoryStream();
                    synthesizer.SetOutputToWaveStream(wavStream);
                    synthesizer.Speak(Text);
                    synthesizer.SetOutputToDefaultAudioDevice();

                    wavStream.Position = 0;
                    using var waveReader = new WaveFileReader(wavStream);
                    using var waveOut = new WaveOutEvent();
                    waveOut.DeviceNumber = deviceNumber;
                    waveOut.Init(waveReader);
                    waveOut.Play();
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
                else
                {
                    // Default device: direct playback for best performance
                    synthesizer.SetOutputToDefaultAudioDevice();
                    synthesizer.Speak(Text);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SpeechSynthesisError", ErrorCategory.InvalidOperation, Text));
            }
        }
    }
}
