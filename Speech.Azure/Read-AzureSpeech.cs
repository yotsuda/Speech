using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;
using Speech.Core;

namespace Speech.Azure
{
    /// <summary>
    /// Continuous speech recognition using Azure Speech Services.
    /// Shows real-time recognition with live updates. Press Enter to confirm and stop.
    /// </summary>
    [Cmdlet(VerbsCommunications.Read, "AzureSpeech")]
    [OutputType(typeof(string), typeof(PSObject[]))]
    public class ReadAzureVoiceCmdlet : AzureCmdlet
    {
        [Parameter]
        public string Language { get; set; } = System.Globalization.CultureInfo.CurrentCulture.Name;

        [Parameter]
        [ValidateRange(1, 300)]
        public int InitialTimeoutSeconds { get; set; } = 30;

        [Parameter]
        [ValidateRange(1, 60)]
        public int EndSilenceSeconds { get; set; } = 3;

        [Parameter]
        public SwitchParameter NoAutoStop { get; set; }

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(MicrophoneCompleter))]
        public string? Microphone { get; set; }

        private readonly List<RecognitionResult> _results = new();
        private readonly object _lock = new();
        private readonly SpinnerDisplay _spinner = new();
        private bool _stopRequested = false;
        private DateTime _lastActivityTime = DateTime.MinValue;
        private bool _hasRecognizedSpeech = false;
        private string _currentHypothesis = "";

        protected override void ProcessRecord()
        {
            try
            {
                var task = RecognizeAsync();
                task.GetAwaiter().GetResult();
                OutputResults();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "AzureSpeechRecognitionError", ErrorCategory.InvalidOperation, null));
            }
        }

        private async Task RecognizeAsync()
        {
            // Ensure UTF-8 output for spinner characters
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            var config = Microsoft.CognitiveServices.Speech.SpeechConfig.FromSubscription(Key!, Region);
            config.SpeechRecognitionLanguage = Language;

            config.SetProperty(PropertyId.SpeechServiceConnection_InitialSilenceTimeoutMs,
                (InitialTimeoutSeconds * 1000).ToString());

            // Resolve microphone from parameter or config
            var microphoneName = ConfigManager.GetMicrophone(Microphone);
            AudioConfig audioConfig;

            if (!string.IsNullOrEmpty(microphoneName))
            {
                var micIndex = MicrophoneCompleter.FindMicrophoneIndex(microphoneName);
                if (micIndex == null)
                {
                    if (!OperatingSystem.IsWindows())
                    {
                        WriteWarning("Microphone selection is only supported on Windows. Using default microphone.");
                    }
                    else
                    {
                        WriteWarning($"Microphone '{microphoneName}' not found. Using default microphone.");
                    }
                    audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                }
                else if (micIndex == 0)
                {
                    // Default microphone - use direct API for better performance
                    audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                    WriteVerbose($"Using default microphone: {microphoneName}");
                }
                else
                {
                    // Non-default microphone - use NAudio stream (Windows only)
                    audioConfig = CreateAudioConfigFromMicrophone(micIndex.Value);
                    WriteVerbose($"Using microphone: {microphoneName}");
                }
            }
            else
            {
                audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            }

            using var _ = audioConfig;
            using var recognizer = new SpeechRecognizer(config, audioConfig);

            // Event handlers
            recognizer.Recognizing += OnRecognizing;
            recognizer.Recognized += OnRecognized;
            recognizer.Canceled += OnCanceled;

            Console.WriteLine($"Starting speech recognition (Language: {Language})");
            if (NoAutoStop)
            {
                Console.WriteLine("Press Enter to confirm...");
            }
            else
            {
                Console.WriteLine($"Press Enter to confirm, or wait {EndSilenceSeconds} seconds of silence for auto-confirm...");
            }
            Console.WriteLine();
            Console.Write("> ");

            await recognizer.StartContinuousRecognitionAsync();

            var startTime = DateTime.Now;
            _lastActivityTime = startTime;

            // Monitor loop
            while (!_stopRequested)
            {
                // Check for key input
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        _stopRequested = true;
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        // Delete last recognized result
                        lock (_lock)
                        {
                            if (_results.Count > 0)
                            {
                                _results.RemoveAt(_results.Count - 1);
                                // Clear current line, move up, clear that line too
                                Console.Write("\r\x1b[2K");  // Clear entire current line
                                Console.Write("\x1b[A");     // Move cursor up
                                Console.Write("\r\x1b[2K");  // Clear entire previous line
                            }
                            _currentHypothesis = "";
                        }
                    }
                }

                // Check auto-stop condition
                if (!NoAutoStop && _hasRecognizedSpeech)
                {
                    lock (_lock)
                    {
                        var silenceDuration = (DateTime.Now - _lastActivityTime).TotalSeconds;
                        if (silenceDuration >= EndSilenceSeconds && string.IsNullOrEmpty(_currentHypothesis))
                        {
                            _stopRequested = true;
                            break;
                        }
                    }
                }

                // Check initial timeout
                if (!_hasRecognizedSpeech)
                {
                    var elapsed = (DateTime.Now - startTime).TotalSeconds;
                    if (elapsed >= InitialTimeoutSeconds)
                    {
                        _stopRequested = true;
                        break;
                    }
                }

                // Update spinner display
                string hypothesis;
                lock (_lock)
                {
                    hypothesis = _currentHypothesis;
                }
                _spinner.DisplaySpinner(hypothesis);

                Thread.Sleep(80);
            }

            await recognizer.StopContinuousRecognitionAsync();
            StopMicrophoneCapture();

            // Clear current line and move to new line
            _spinner.ClearCurrentLine();
            Console.WriteLine();
            Console.CursorVisible = true;

            // OutputResults moved to ProcessRecord
        }

        private void OnRecognizing(object? sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizingSpeech && !string.IsNullOrWhiteSpace(e.Result.Text))
            {
                lock (_lock)
                {
                    _currentHypothesis = e.Result.Text;
                    _lastActivityTime = DateTime.Now;
                    _hasRecognizedSpeech = true;
                }

                // Display handled by main loop spinner
            }
        }

        private void OnRecognized(object? sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(e.Result.Text))
            {
                lock (_lock)
                {
                    _results.Add(new RecognitionResult
                    {
                        Text = e.Result.Text,
                        Duration = e.Result.Duration.TotalSeconds,
                        Timestamp = DateTime.Now
                    });

                    _currentHypothesis = "";
                    _lastActivityTime = DateTime.Now;
                    _hasRecognizedSpeech = true;
                }

                // Finalize current line and start new line
                _spinner.DisplayFinal(e.Result.Text);
            }
        }

        private void OnCanceled(object? sender, SpeechRecognitionCanceledEventArgs e)
        {
            if (e.Reason == CancellationReason.Error)
            {
                Console.WriteLine();
                Console.WriteLine($"Recognition error: {e.ErrorCode} - {e.ErrorDetails}");
            }
            _stopRequested = true;
        }

        private void OutputResults()
        {
            if (_results.Count == 0)
            {
                Console.WriteLine("No recognition results.");
                WriteObject(null);
                return;
            }

            if (PassThru)
            {
                var output = new List<PSObject>();
                foreach (var result in _results)
                {
                    var obj = new PSObject();
                    obj.Properties.Add(new PSNoteProperty("Text", result.Text));
                    obj.Properties.Add(new PSNoteProperty("Duration", result.Duration));
                    obj.Properties.Add(new PSNoteProperty("Timestamp", result.Timestamp));
                    output.Add(obj);
                }
                WriteObject(output.ToArray());
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in _results)
                {
                    if (sb.Length > 0) sb.Append(' ');
                    sb.Append(result.Text);
                }
                WriteObject(sb.ToString());
            }
        }

        private class RecognitionResult
        {
            public string Text { get; set; } = "";
            public double Duration { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private WaveInEvent? _waveIn;
        private PushAudioInputStream? _audioInputStream;

        private AudioConfig CreateAudioConfigFromMicrophone(int deviceIndex)
        {
            // Create push stream for Azure Speech SDK
            var format = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1);
            _audioInputStream = AudioInputStream.CreatePushStream(format);

            // Create NAudio WaveIn for specific microphone
            _waveIn = new WaveInEvent
            {
                DeviceNumber = deviceIndex,
                WaveFormat = new WaveFormat(16000, 16, 1)
            };

            _waveIn.DataAvailable += (sender, e) =>
            {
                if (e.BytesRecorded > 0)
                {
                    _audioInputStream.Write(e.Buffer, e.BytesRecorded);
                }
            };

            _waveIn.RecordingStopped += (sender, e) =>
            {
                _audioInputStream?.Close();
            };

            _waveIn.StartRecording();

            return AudioConfig.FromStreamInput(_audioInputStream);
        }

        private void StopMicrophoneCapture()
        {
            _waveIn?.StopRecording();
            _waveIn?.Dispose();
            _waveIn = null;
        }
    }
}
