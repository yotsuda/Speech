using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading;
using NAudio.Wave;
using Speech.Core;

namespace Speech.Amazon
{
    [Cmdlet(VerbsCommunications.Read, "AmazonSpeech")]
    [OutputType(typeof(string))]
    public class ReadAmazonSpeechCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(AmazonLanguageCompleter))]
        public string? Language { get; set; }

        [Parameter]
        public string? AccessKey { get; set; }

        [Parameter]
        public string? SecretKey { get; set; }

        [Parameter]
        public string? Region { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(MicrophoneCompleter))]
        public string? Microphone { get; set; }

        [Parameter]
        [ValidateRange(1, 120)]
        public int InitialTimeoutSeconds { get; set; } = 30;

        [Parameter]
        [ValidateRange(1, 30)]
        public int EndSilenceSeconds { get; set; } = 3;

        [Parameter]
        public SwitchParameter NoAutoStop { get; set; }

        private readonly SpinnerDisplay _spinner = new();
        private WaveInEvent? _waveIn;
        private MemoryStream? _audioStream;
        private BinaryWriter? _audioWriter;
        private bool _hasSound;
        private DateTime _silenceStart = DateTime.MinValue;
        private bool _stopRequested;
        private string? _accessKey;
        private string? _secretKey;
        private string? _region;
        private string? _language;

        protected override void ProcessRecord()
        {
            var config = ConfigManager.GetConfig();

            _accessKey = AccessKey ?? config.Amazon?.AccessKey;
            _secretKey = SecretKey ?? config.Amazon?.SecretKey;
            _region = Region ?? config.Amazon?.Region;

            if (string.IsNullOrEmpty(_accessKey) || string.IsNullOrEmpty(_secretKey))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException(
                        "Amazon credentials not specified. Use -AccessKey/-SecretKey or Set-AmazonSpeechConfig -AccessKey <key> -SecretKey <key>"),
                    "NoCredential",
                    ErrorCategory.InvalidOperation,
                    null));
                return;
            }

            if (string.IsNullOrEmpty(_region))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException(
                        "Amazon region not specified. Use -Region or Set-AmazonSpeechConfig -Region <region>"),
                    "NoRegion",
                    ErrorCategory.InvalidOperation,
                    null));
                return;
            }

            _language = Language ?? config.Common?.Language ?? "ja-JP";

            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.CursorVisible = false;

                var audioData = RecordAudio(config);
                Console.CursorVisible = true;

                if (audioData == null || audioData.Length == 0)
                {
                    Console.WriteLine("\nNo audio recorded");
                    return;
                }

                // Show transcribing spinner
                Console.Write("\r> Transcribing... ");

                using var manager = new AmazonAudioManager(_accessKey, _secretKey, _region);
                using var audioMs = new MemoryStream(audioData);
                var text = manager.TranscribeStreamAsync(audioMs, _language).GetAwaiter().GetResult();

                // Clear spinner line
                Console.Write("\r" + new string(' ', 50) + "\r");

                if (!string.IsNullOrEmpty(text))
                {
                    WriteObject(text.Trim());
                }
                else
                {
                    Console.WriteLine("\r> (no speech detected)");
                }
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }

        private byte[]? RecordAudio(SpeechConfig config)
        {
            var micName = Microphone ?? config.Common?.Microphone;
            var deviceNumber = MicrophoneHelper.GetDeviceNumber(micName);

            _audioStream = new MemoryStream();
            _audioWriter = new BinaryWriter(_audioStream);
            _hasSound = false;
            _stopRequested = false;
            _silenceStart = DateTime.MinValue;

            _waveIn = new WaveInEvent
            {
                DeviceNumber = deviceNumber,
                WaveFormat = new WaveFormat(16000, 16, 1) // 16kHz, 16-bit, mono
            };

            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.StartRecording();

            Console.WriteLine($"Starting speech recognition (Language: {_language})");
            if (NoAutoStop)
            {
                Console.WriteLine("Press Enter to confirm...");
            }
            else
            {
                Console.WriteLine($"Press Enter to confirm, or wait {EndSilenceSeconds} seconds of silence for auto-confirm...");
            }
            Console.WriteLine();

            var startTime = DateTime.Now;

            // Monitor loop with spinner
            while (!_stopRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }

                // Check auto-stop condition
                if (!NoAutoStop && _hasSound && _silenceStart != DateTime.MinValue)
                {
                    var silenceDuration = (DateTime.Now - _silenceStart).TotalSeconds;
                    if (silenceDuration >= EndSilenceSeconds)
                    {
                        break;
                    }
                }

                // Check initial timeout
                if (!_hasSound)
                {
                    var elapsed = (DateTime.Now - startTime).TotalSeconds;
                    if (elapsed >= InitialTimeoutSeconds)
                    {
                        break;
                    }
                }

                // Show spinner
                _spinner.DisplaySpinner(_hasSound ? "Recording..." : "");

                Thread.Sleep(80);
            }

            _waveIn.StopRecording();
            _waveIn.DataAvailable -= OnDataAvailable;
            _waveIn.Dispose();

            _spinner.ClearCurrentLine();

            return _audioStream.ToArray();
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            _audioWriter?.Write(e.Buffer, 0, e.BytesRecorded);

            // Detect sound using max amplitude of entire buffer
            var maxAmplitude = 0;
            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                var sample = Math.Abs(BitConverter.ToInt16(e.Buffer, i));
                if (sample > maxAmplitude) maxAmplitude = sample;
            }

            if (maxAmplitude > 1500)
            {
                _hasSound = true;
                _silenceStart = DateTime.MinValue;
            }
            else if (_hasSound && _silenceStart == DateTime.MinValue)
            {
                _silenceStart = DateTime.Now;
            }
        }
    }
}
