using System.Management.Automation;

namespace Speech.Core
{
    /// <summary>
    /// Set common speech configuration settings.
    /// For provider-specific settings, use Set-AzureSpeechConfig, Set-OpenAISpeechConfig, etc.
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
        [ArgumentCompleter(typeof(OutputDeviceCompleter))]
        public string? OutputDevice { get; set; }

        [Parameter]
        public string? Language { get; set; }

        protected override void ProcessRecord()
        {
            bool updated = false;

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

            if (!string.IsNullOrEmpty(OutputDevice))
            {
                ConfigManager.UpdateOutputDeviceIfSpecified(OutputDevice);
                WriteVerbose($"OutputDevice set to: {OutputDevice}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Language))
            {
                var clearedVoices = ConfigManager.UpdateLanguageIfSpecified(Language);
                WriteVerbose($"Language set to: {Language}");
                updated = true;

                foreach (var cleared in clearedVoices)
                {
                    WriteWarning($"Cleared {cleared} because it conflicts with Language '{Language}'.");
                }
            }

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Rate, -Volume, -OutputDevice, -Language, or -Microphone. For provider-specific settings, use Set-AzureSpeechConfig, Set-OpenAISpeechConfig, Set-GoogleSpeechConfig, or Set-WindowsSpeechConfig.");
            }
        }
    }
}
