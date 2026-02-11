using System.Management.Automation;
using System.Text;
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
        [ArgumentCompleter(typeof(OpenAILanguageCompleter))]
        public string Language { get; set; } = System.Globalization.CultureInfo.CurrentCulture.Name;

        [Parameter]
        [ArgumentCompleter(typeof(MicrophoneCompleter))]
        public string? Microphone { get; set; }

        [Parameter]
        [ValidateRange(1, 300)]
        public int InitialTimeoutSeconds { get; set; } = 30;

        [Parameter]
        [ValidateRange(1, 60)]
        public int EndSilenceSeconds { get; set; } = 3;

        [Parameter]
        public SwitchParameter NoAutoStop { get; set; }

        private readonly SpinnerDisplay _spinner = new();
        private bool _stopRequested = false;
        private bool _hasSound = false;
        private DateTime _silenceStart = DateTime.MinValue;

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

            var language = Language ?? config.Common?.Language ?? System.Globalization.CultureInfo.CurrentCulture.Name;

            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.CursorVisible = false;

                var audioData = RecordAudio();

                Console.CursorVisible = true;

                if (audioData == null || audioData.Length == 0)
                {
                    Console.WriteLine("\nNo audio recorded");
                    return;
                }

                // Show transcribing spinner
                Console.Write("\r> Transcribing... ");
                
                using var manager = new OpenAIAudioManager(apiKey);
                var text = manager.SpeechToTextAsync(audioData, "audio.wav", "whisper-1", language)
                    .GetAwaiter().GetResult();

                // Clear the "Transcribing..." line
                Console.Write("\r" + new string(' ', 50) + "\r");

                if (!string.IsNullOrWhiteSpace(text))
                {
                    WriteObject(text.Trim());
                }
                else
                {
                    Console.WriteLine("\r> (no speech detected)");
                }
            }
            catch (Exception ex)
            {
                Console.CursorVisible = true;
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

            _silenceStart = DateTime.MinValue;
            _hasSound = false;
            _stopRequested = false;
            var startTime = DateTime.Now;
            var recordingComplete = false;

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
                    _hasSound = true;
                    _silenceStart = DateTime.MinValue;
                }
                else if (_hasSound && _silenceStart == DateTime.MinValue)
                {
                    _silenceStart = DateTime.Now;
                }
            };

            // Show initial message
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

            waveIn.StartRecording();

            // Monitor loop with spinner
            while (!recordingComplete && !_stopRequested)
            {
                // Check for Enter key
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        _stopRequested = true;
                        break;
                    }
                }

                // Show spinner
                _spinner.DisplaySpinner(_hasSound ? "Recording..." : "");

                // Check auto-stop condition
                if (!NoAutoStop && _hasSound && _silenceStart != DateTime.MinValue)
                {
                    var silenceDuration = (DateTime.Now - _silenceStart).TotalSeconds;
                    if (silenceDuration >= EndSilenceSeconds)
                    {
                        recordingComplete = true;
                        break;
                    }
                }

                // Check timeout
                if ((DateTime.Now - startTime).TotalSeconds > InitialTimeoutSeconds)
                {
                    recordingComplete = true;
                    break;
                }

                Thread.Sleep(100);
            }

            waveIn.StopRecording();
            writer.Flush();

            return ms.ToArray();
        }
    }
}
