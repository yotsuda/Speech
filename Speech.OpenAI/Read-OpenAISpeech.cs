using System.Management.Automation;
using Speech.Core;
using NAudio.Wave;

namespace Speech.OpenAI
{
    [Cmdlet(VerbsCommunications.Read, "OpenAISpeech")]
    [OutputType(typeof(string))]
    public class ReadOpenAISpeechCmdlet : PSCmdlet
    {
        [Parameter]
        public string? ApiKey { get; set; }

        [Parameter]
        public string? Language { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(MicrophoneCompleter))]
        public string? Microphone { get; set; }

        [Parameter]
        [ValidateRange(1, 60)]
        public int TimeoutSeconds { get; set; } = 30;

        [Parameter]
        [ValidateRange(0.5, 5.0)]
        public double SilenceThreshold { get; set; } = 2.0;

        protected override void ProcessRecord()
        {
            if (!OperatingSystem.IsWindows())
            {
                WriteError(new ErrorRecord(
                    new PlatformNotSupportedException("Read-OpenAISpeech requires Windows for microphone access"),
                    "WindowsOnly",
                    ErrorCategory.InvalidOperation,
                    null));
                return;
            }

            var config = ConfigManager.GetConfig();

            var apiKey = ApiKey ?? config.OpenAI?.Key;
            if (string.IsNullOrEmpty(apiKey))
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException("OpenAI API key not configured. Use -ApiKey or Set-SpeechConfig -OpenAIKey"),
                    "NoApiKey",
                    ErrorCategory.AuthenticationError,
                    null));
                return;
            }

            var language = Language ?? config.Common?.Language;

            try
            {
                var audioData = RecordAudio();
                if (audioData == null || audioData.Length == 0)
                {
                    WriteWarning("No audio recorded");
                    return;
                }

                Host.UI.Write("Transcribing...");

                using var manager = new OpenAIAudioManager(apiKey);
                var text = manager.SpeechToTextAsync(audioData, "audio.wav", "whisper-1", language)
                    .GetAwaiter().GetResult();

                Host.UI.WriteLine(" Done");

                if (!string.IsNullOrWhiteSpace(text))
                {
                    WriteObject(text.Trim());
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "STTError", ErrorCategory.ConnectionError, null));
            }
        }

        private byte[]? RecordAudio()
        {
            var config = ConfigManager.GetConfig();
            var micName = Microphone ?? config.Common?.Microphone;

            int deviceNumber = -1;
            if (!string.IsNullOrEmpty(micName))
            {
                for (int i = 0; i < WaveInEvent.DeviceCount; i++)
                {
                    var caps = WaveInEvent.GetCapabilities(i);
                    if (caps.ProductName.Contains(micName, StringComparison.OrdinalIgnoreCase))
                    {
                        deviceNumber = i;
                        break;
                    }
                }
            }

            using var waveIn = new WaveInEvent
            {
                DeviceNumber = deviceNumber >= 0 ? deviceNumber : 0,
                WaveFormat = new WaveFormat(16000, 16, 1)
            };

            using var ms = new MemoryStream();
            using var writer = new WaveFileWriter(ms, waveIn.WaveFormat);

            var silenceStart = DateTime.MinValue;
            var hasSound = false;
            var recordingComplete = false;
            var startTime = DateTime.Now;

            waveIn.DataAvailable += (s, e) =>
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);

                var maxAmplitude = 0;
                for (int i = 0; i < e.BytesRecorded; i += 2)
                {
                    var sample = Math.Abs(BitConverter.ToInt16(e.Buffer, i));
                    if (sample > maxAmplitude) maxAmplitude = sample;
                }

                if (maxAmplitude > 500)
                {
                    hasSound = true;
                    silenceStart = DateTime.MinValue;
                }
                else if (hasSound && silenceStart == DateTime.MinValue)
                {
                    silenceStart = DateTime.Now;
                }

                if (hasSound && silenceStart != DateTime.MinValue &&
                    (DateTime.Now - silenceStart).TotalSeconds > SilenceThreshold)
                {
                    recordingComplete = true;
                }

                if ((DateTime.Now - startTime).TotalSeconds > TimeoutSeconds)
                {
                    recordingComplete = true;
                }
            };

            Host.UI.WriteLine("🎤 Listening... (speak, then pause to finish)");
            waveIn.StartRecording();

            while (!recordingComplete)
            {
                Thread.Sleep(100);
            }

            waveIn.StopRecording();
            writer.Flush();

            return ms.ToArray();
        }
    }
}
