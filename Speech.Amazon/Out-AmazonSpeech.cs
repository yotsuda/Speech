using System;
using System.IO;
using System.Management.Automation;
using NAudio.Wave;
using Speech.Core;

namespace Speech.Amazon
{
    [Cmdlet(VerbsData.Out, "AmazonSpeech")]
    public class OutAmazonSpeechCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string Text { get; set; } = "";

        [Parameter]
        [ArgumentCompleter(typeof(AmazonSpeechCompleter))]
        public string? Voice { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(AmazonLanguageCompleter))]
        public string? Language { get; set; }

        [Parameter]
        [ValidateRange(0.25, 4.0)]
        public double? Rate { get; set; }

        [Parameter]
        public string? AccessKey { get; set; }

        [Parameter]
        public string? SecretKey { get; set; }

        [Parameter]
        public string? Region { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(OutputDeviceCompleter))]
        public string? OutputDevice { get; set; }

        private string? _accessKey;
        private string? _secretKey;
        private string? _region;
        private string? _voice;
        private string? _language;

        protected override void BeginProcessing()
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

            // Voice
            _voice = Voice ?? config.Amazon?.Voice ?? GetDefaultVoice();

            // Language - derive from config if not specified
            _language = Language ?? config.Common?.Language;
        }

        protected override void ProcessRecord()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            try
            {
                using var manager = new AmazonAudioManager(_accessKey!, _secretKey!, _region!);
                var rate = Rate.HasValue ? ConfigManager.GetRate(Rate) : ConfigManager.GetRate(null);
                var audioData = manager.TextToSpeechAsync(Text, _voice!, _language, rate).GetAwaiter().GetResult();
                var outputDeviceName = ConfigManager.GetOutputDevice(OutputDevice);
                var deviceNumber = OutputDeviceCompleter.FindOutputDeviceIndex(outputDeviceName) ?? -1;
                PlayMp3Audio(audioData, deviceNumber);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "AmazonTTSError", ErrorCategory.InvalidOperation, Text));
            }
        }

        internal static string GetDefaultVoice()
        {
            var locale = System.Globalization.CultureInfo.CurrentUICulture.Name;
            var lang = locale.Split('-')[0].ToLowerInvariant();
            return lang switch
            {
                "ja" => "Mizuki",
                "ko" => "Seoyeon",
                "zh" => "Zhiyu",
                "de" => "Vicki",
                "fr" => "Lea",
                "es" => "Lucia",
                "it" => "Bianca",
                "pt" => "Camila",
                _ => "Joanna"
            };
        }

        private void PlayMp3Audio(byte[] audioData, int deviceNumber = -1)
        {
            using var ms = new MemoryStream(audioData);
            using var reader = new Mp3FileReader(ms);
            using var waveOut = new WaveOutEvent();
            waveOut.DeviceNumber = deviceNumber;
            waveOut.Init(reader);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
