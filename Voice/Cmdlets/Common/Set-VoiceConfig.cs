using System.Management.Automation;

namespace Voice.Cmdlets.Common
{
    /// <summary>
    /// Set voice configuration settings
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "VoiceConfig")]
    public class SetVoiceConfigCmdlet : PSCmdlet
    {
        [Parameter]
        [ValidateRange(0.5, 2.0)]
        public double? Rate { get; set; }

        [Parameter]
        [ValidateRange(0, 100)]
        public int? Volume { get; set; }

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

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Rate, -Volume, -WindowsVoice, -AzureVoice, -AzurePitch, -AzureKey, or -AzureRegion.");
            }
        }
    }
}
