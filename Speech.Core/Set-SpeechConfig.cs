using System.Management.Automation;

namespace Speech.Core
{
    /// <summary>
    /// Set voice configuration settings
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "SpeechConfig")]
    public class SetSpeechConfigCmdlet : PSCmdlet
    {
        [Parameter]
        [ValidateRange(0.5, 2.0)]
        public double? Rate { get; set; }

        [Parameter]
        [ValidateRange(0, 100)]
        public int? Volume { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(MicrophoneCompleter))]
        public string? Microphone { get; set; }

        [Parameter]
        public string? Language { get; set; }

        [Parameter]
        public string? WindowsVoice { get; set; }

        [Parameter]
        public string? AzureVoice { get; set; }

        [Parameter]
        [ValidateRange(-50, 50)]
        public int? AzurePitch { get; set; }

        [Parameter]
        public string? AzureKey { get; set; }

        [Parameter]
        public string? AzureRegion { get; set; }

        [Parameter]
        public string? OpenAIKey { get; set; }

        [Parameter]
        [ValidateSet("alloy", "ash", "ballad", "coral", "echo", "fable",
                     "onyx", "nova", "sage", "shimmer", "verse")]
        public string? OpenAIVoice { get; set; }

        [Parameter]
        [ValidateSet("tts-1", "tts-1-hd", "gpt-4o-mini-tts")]
        public string? OpenAIModel { get; set; }

        [Parameter]
        public string? GoogleCredential { get; set; }

        [Parameter]
        public string? GoogleVoice { get; set; }

        protected override void ProcessRecord()
        {
            bool updated = false;

            // Common settings
            if (Rate.HasValue)
            {
                ConfigManager.UpdateRateIfSpecified(Rate);
                WriteVerbose($"Rate set to: {Rate}");
                updated = true;
            }

            if (Volume.HasValue)
            {
                ConfigManager.UpdateVolumeIfSpecified(Volume);
                WriteVerbose($"Volume set to: {Volume}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Microphone))
            {
                ConfigManager.UpdateMicrophoneIfSpecified(Microphone);
                WriteVerbose($"Microphone set to: {Microphone}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Language))
            {
                var clearedVoices = ConfigManager.UpdateLanguageIfSpecified(Language);
                WriteVerbose($"Language set to: {Language}");
                updated = true;

                // Warn about cleared conflicting voice settings
                foreach (var cleared in clearedVoices)
                {
                    WriteWarning($"Cleared {cleared} because it conflicts with Language '{Language}'.");
                }
            }

            // Windows settings
            if (!string.IsNullOrEmpty(WindowsVoice))
            {
                ConfigManager.UpdateWindowsVoiceIfSpecified(WindowsVoice);
                WriteVerbose($"Windows voice set to: {WindowsVoice}");
                updated = true;
            }

            // Azure settings
            if (!string.IsNullOrEmpty(AzureVoice))
            {
                ConfigManager.UpdateAzureVoiceIfSpecified(AzureVoice);
                WriteVerbose($"Azure voice set to: {AzureVoice}");
                updated = true;
            }

            if (AzurePitch.HasValue)
            {
                ConfigManager.UpdateAzurePitchIfSpecified(AzurePitch);
                WriteVerbose($"Azure pitch set to: {AzurePitch}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(AzureKey))
            {
                ConfigManager.UpdateAzureKeyIfSpecified(AzureKey);
                WriteVerbose("Azure key updated");
                updated = true;
            }

            if (!string.IsNullOrEmpty(AzureRegion))
            {
                ConfigManager.UpdateAzureRegionIfSpecified(AzureRegion);
                WriteVerbose($"Azure region set to: {AzureRegion}");
                updated = true;
            }

            // OpenAI settings
            if (!string.IsNullOrEmpty(OpenAIKey))
            {
                ConfigManager.UpdateOpenAIKeyIfSpecified(OpenAIKey);
                WriteVerbose("OpenAI key updated");
                updated = true;
            }

            if (!string.IsNullOrEmpty(OpenAIVoice))
            {
                ConfigManager.UpdateOpenAIVoiceIfSpecified(OpenAIVoice);
                WriteVerbose($"OpenAI voice set to: {OpenAIVoice}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(OpenAIModel))
            {
                ConfigManager.UpdateOpenAIModelIfSpecified(OpenAIModel);
                WriteVerbose($"OpenAI model set to: {OpenAIModel}");
                updated = true;
            }

            // Google settings
            if (!string.IsNullOrEmpty(GoogleCredential))
            {
                ConfigManager.UpdateGoogleCredentialIfSpecified(GoogleCredential);
                WriteVerbose($"Google credential set to: {GoogleCredential}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(GoogleVoice))
            {
                ConfigManager.UpdateGoogleVoiceIfSpecified(GoogleVoice);
                WriteVerbose($"Google voice set to: {GoogleVoice}");
                updated = true;
            }

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Rate, -Volume, -Language, -WindowsVoice, -AzureVoice, -AzurePitch, -AzureKey, -AzureRegion, -OpenAIKey, -OpenAIVoice, -OpenAIModel, -GoogleCredential, or -GoogleVoice.");
            }
        }
    }
}
