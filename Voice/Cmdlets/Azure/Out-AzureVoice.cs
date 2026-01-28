using System.Management.Automation;
using Voice.Cmdlets.Common;

namespace Voice.Cmdlets.Azure
{
    [Cmdlet(VerbsData.Out, "AzureVoice")]
    public class OutAzureVoiceCmdlet : AzureCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string? Text { get; set; }

        [Parameter]
        public string? Voice { get; set; }

        [Parameter]
        [ValidateRange(0.5, 2.0)]
        public double? Rate { get; set; }

        [Parameter]
        [ValidateRange(0, 100)]
        public int? Volume { get; set; }

        [Parameter]
        [ValidateRange(-50, 50)]
        public int? Pitch { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var config = ConfigManager.GetConfig();

                var voice = Voice ?? config.Azure?.Voice ?? "en-US-JennyNeural";
                var rate = Rate ?? config.Common?.Rate ?? 1.0;
                var volume = Volume ?? config.Common?.Volume ?? 100;
                var pitch = Pitch ?? config.Azure?.Pitch ?? 0;

                var task = SynthesizeSpeechAsync(voice, rate, volume, pitch);
                task.GetAwaiter().GetResult();


            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Invalid audio format") || ex.Message.Contains("empty audio data"))
            {
                // Check if this might be due to language mismatch
                var config = ConfigManager.GetConfig();
                var voice = Voice ?? config.Azure?.Voice ?? "en-US-JennyNeural";
                var textLanguage = DetectTextLanguage(Text);
                var voiceLanguage = ExtractVoiceLanguage(voice);

                string errorMessage = ex.Message;

                // Add language mismatch hint if detected
                if (textLanguage != null && voiceLanguage != null && textLanguage != voiceLanguage)
                {
                    errorMessage += System.Environment.NewLine + System.Environment.NewLine +
                        $"💡 Language mismatch detected:" + System.Environment.NewLine +
                        $"   Text language: {textLanguage}" + System.Environment.NewLine +
                        $"   Voice language: {voiceLanguage} ({voice})" + System.Environment.NewLine +
                        $"   Try using a {textLanguage} voice for this text.";
                }

                WriteError(new ErrorRecord(
                    new InvalidOperationException(errorMessage, ex),
                    "AzureSpeechAudioError",
                    ErrorCategory.InvalidOperation,
                    Text));
            }
            catch (InvalidOperationException ex)
            {
                // Handle other Azure-specific errors
                WriteError(new ErrorRecord(
                    ex,
                    "AzureSpeechError",
                    ErrorCategory.InvalidOperation,
                    Text));
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                WriteError(new ErrorRecord(
                    new Exception($"Unexpected error during speech synthesis: {ex.Message}", ex),
                    "UnexpectedSpeechError",
                    ErrorCategory.NotSpecified,
                    Text));
                WriteVerbose($"Full exception details:" + System.Environment.NewLine + ex);
            }
        }

        private async Task SynthesizeSpeechAsync(string voice, double rate, int volume, int pitch)
        {
            var manager = AzureAudioManager.GetInstance(Key!, Region!);
            var ssml = BuildSsml(Text!, voice, rate, volume, pitch);
            await manager.SynthesizeSpeechAsync(ssml);
        }

        private string BuildSsml(string text, string voice, double rate, int volume, int pitch)
        {
            // Extract language from voice name (e.g., "ja-JP-NanamiNeural" -> "ja-JP")
            var lang = "en-US"; // default
            if (voice.Contains("-"))
            {
                var parts = voice.Split('-');
                if (parts.Length >= 2)
                {
                    lang = $"{parts[0]}-{parts[1]}";
                }
            }

            int ratePercent = (int)Math.Round((rate - 1.0) * 100);
            var rateStr = ratePercent >= 0 ? $"+{ratePercent}%" : $"{ratePercent}%";
            var volumeStr = $"{volume}%";

            var prosodyAttrs = $"rate=\"{rateStr}\" volume=\"{volumeStr}\"";
            if (pitch != 0)
            {
                var pitchStr = pitch >= 0 ? $"+{pitch}Hz" : $"{pitch}Hz";
                prosodyAttrs += $" pitch=\"{pitchStr}\"";
            }

            return $@"<speak version=""1.0"" xml:lang=""{lang}"" xmlns=""http://www.w3.org/2001/10/synthesis"">
    <voice name=""{voice}"">
        <prosody {prosodyAttrs}>
            {System.Security.SecurityElement.Escape(text)}
        </prosody>
    </voice>
</speak>";
        }

        /// <summary>
        /// Attempts to detect the language of the text
        /// </summary>
        private string? DetectTextLanguage(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            foreach (char c in text)
            {
                if ((c >= 0x3040 && c <= 0x309F) || (c >= 0x30A0 && c <= 0x30FF)) return "ja-JP"; // Hiragana/Katakana
                if (c >= 0x4E00 && c <= 0x9FFF) return "ja-JP"; // CJK (default to Japanese)
                if (c >= 0xAC00 && c <= 0xD7AF) return "ko-KR"; // Hangul
            }

            return "en-US";
        }

        /// <summary>
        /// Extracts the language code from voice name (e.g., "ja-JP-NanamiNeural" -> "ja-JP")
        /// </summary>
        private string? ExtractVoiceLanguage(string? voice)
        {
            if (string.IsNullOrEmpty(voice))
                return null;

            var parts = voice.Split('-');
            if (parts.Length >= 2)
            {
                return $"{parts[0]}-{parts[1]}";
            }

            return null;
        }
    }
}
