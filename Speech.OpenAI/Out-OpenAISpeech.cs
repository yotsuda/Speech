using System.Management.Automation;
using Speech.Core;
using NAudio.Wave;

namespace Speech.OpenAI
{
    [Cmdlet(VerbsData.Out, "OpenAISpeech")]
    public class OutOpenAISpeechCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string? Text { get; set; }

        [Parameter]
        [ValidateSet("alloy", "ash", "ballad", "coral", "echo", "fable",
                     "onyx", "nova", "sage", "shimmer", "verse")]
        public string? Voice { get; set; }

        [Parameter]
        [ValidateSet("tts-1", "tts-1-hd", "gpt-4o-mini-tts")]
        public string? Model { get; set; }

        [Parameter]
        [ValidateRange(0.25, 4.0)]
        public double? Speed { get; set; }

        [Parameter]
        public string? ApiKey { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(OutputDeviceCompleter))]
        public string? OutputDevice { get; set; }

        protected override void ProcessRecord()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

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

            var voice = Voice ?? config.OpenAI?.Voice ?? "alloy";
            var model = Model ?? config.OpenAI?.Model ?? "tts-1";
            var speed = Speed ?? ConfigManager.GetRate(null, 1.0);

            try
            {
                using var manager = new OpenAIAudioManager(apiKey);
                var audioBytes = manager.TextToSpeechAsync(Text, voice, model, speed)
                    .GetAwaiter().GetResult();

                var outputDeviceName = ConfigManager.GetOutputDevice(OutputDevice);
                var deviceNumber = OutputDeviceCompleter.FindOutputDeviceIndex(outputDeviceName) ?? -1;

                using var ms = new MemoryStream(audioBytes);
                using var mp3Reader = new Mp3FileReader(ms);
                using var waveOut = new WaveOutEvent();
                waveOut.DeviceNumber = deviceNumber;
                waveOut.Init(mp3Reader);
                waveOut.Play();

                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "TTSError", ErrorCategory.ConnectionError, Text));
            }
        }
    }
}
