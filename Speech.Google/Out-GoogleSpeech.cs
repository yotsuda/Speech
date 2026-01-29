using System;
using System.IO;
using System.Management.Automation;
using NAudio.Wave;
using Speech.Core;

namespace Speech.Google
{
    [Cmdlet(VerbsData.Out, "GoogleSpeech")]
    public class OutGoogleSpeechCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string Text { get; set; } = "";

        [Parameter]
        public string? Voice { get; set; }

        [Parameter]
        public string? Language { get; set; }

        [Parameter]
        [ValidateRange(0.25, 4.0)]
        public double? Rate { get; set; }

        [Parameter]
        [ValidateRange(-20.0, 20.0)]
        public double? Pitch { get; set; }

        [Parameter]
        public string? Credential { get; set; }

        private string? _credentialPath;
        private string? _voice;
        private string? _language;

        protected override void BeginProcessing()
        {
            var config = ConfigManager.GetConfig();

            // Credential path
            _credentialPath = Credential ?? config.Google?.Credential;
            if (string.IsNullOrEmpty(_credentialPath))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException("Google credential path not specified. Use -Credential or Set-SpeechConfig -GoogleCredential"),
                    "NoCredential",
                    ErrorCategory.InvalidOperation,
                    null));
                return;
            }

            if (!File.Exists(_credentialPath))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new FileNotFoundException($"Credential file not found: {_credentialPath}"),
                    "CredentialNotFound",
                    ErrorCategory.ObjectNotFound,
                    _credentialPath));
                return;
            }

            // Voice
            _voice = Voice ?? config.Google?.Voice ?? "en-US-Standard-A";

            // Language - extract from voice name if not specified
            _language = Language ?? config.Common?.Language;
            if (string.IsNullOrEmpty(_language) && !string.IsNullOrEmpty(_voice))
            {
                // Voice format: "en-US-Standard-A" -> "en-US"
                var parts = _voice.Split('-');
                if (parts.Length >= 2)
                {
                    _language = $"{parts[0]}-{parts[1]}";
                }
            }
            _language ??= "en-US";
        }

        protected override void ProcessRecord()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            try
            {
                using var manager = new GoogleAudioManager(_credentialPath!);
                var audioData = manager.TextToSpeechAsync(Text, _voice!, _language!, Rate, Pitch).GetAwaiter().GetResult();
                PlayMp3Audio(audioData);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GoogleTTSError", ErrorCategory.InvalidOperation, Text));
            }
        }

        private void PlayMp3Audio(byte[] audioData)
        {
            using var ms = new MemoryStream(audioData);
            using var reader = new Mp3FileReader(ms);
            using var waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
