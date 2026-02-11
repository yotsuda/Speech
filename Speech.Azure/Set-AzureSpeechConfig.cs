using System.Management.Automation;
using Speech.Core;

namespace Speech.Azure
{
    [Cmdlet(VerbsCommon.Set, "AzureSpeechConfig")]
    public class SetAzureSpeechConfigCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(AzureSpeechCompleter))]
        public string? Voice { get; set; }

        [Parameter]
        [ValidateRange(-50, 50)]
        public int? Pitch { get; set; }

        [Parameter]
        public string? Key { get; set; }

        [Parameter]
        public string? Region { get; set; }

        protected override void ProcessRecord()
        {
            bool updated = false;

            if (!string.IsNullOrEmpty(Voice))
            {
                ConfigManager.UpdateAzureVoiceIfSpecified(Voice);
                WriteVerbose($"Azure voice set to: {Voice}");
                updated = true;
            }

            if (Pitch.HasValue)
            {
                ConfigManager.UpdateAzurePitchIfSpecified(Pitch);
                WriteVerbose($"Azure pitch set to: {Pitch}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Key))
            {
                ConfigManager.UpdateAzureKeyIfSpecified(Key);
                WriteVerbose("Azure key updated");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Region))
            {
                ConfigManager.UpdateAzureRegionIfSpecified(Region);
                WriteVerbose($"Azure region set to: {Region}");
                updated = true;
            }

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Voice, -Pitch, -Key, or -Region.");
            }
        }
    }
}
