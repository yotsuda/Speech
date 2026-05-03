using System.Management.Automation;
using Speech.Core;

namespace Speech.Amazon
{
    [Cmdlet(VerbsCommon.Set, "AmazonSpeechConfig")]
    public class SetAmazonSpeechConfigCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(AmazonSpeechCompleter))]
        public string? Voice { get; set; }

        [Parameter]
        public string? AccessKey { get; set; }

        [Parameter]
        public string? SecretKey { get; set; }

        [Parameter]
        public string? Region { get; set; }

        protected override void ProcessRecord()
        {
            bool updated = false;

            if (!string.IsNullOrEmpty(Voice))
            {
                ConfigManager.UpdateAmazonVoiceIfSpecified(Voice);
                WriteVerbose($"Amazon voice set to: {Voice}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(AccessKey))
            {
                ConfigManager.UpdateAmazonAccessKeyIfSpecified(AccessKey);
                WriteVerbose("Amazon access key updated");
                updated = true;
            }

            if (!string.IsNullOrEmpty(SecretKey))
            {
                ConfigManager.UpdateAmazonSecretKeyIfSpecified(SecretKey);
                WriteVerbose("Amazon secret key updated");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Region))
            {
                ConfigManager.UpdateAmazonRegionIfSpecified(Region);
                WriteVerbose($"Amazon region set to: {Region}");
                updated = true;
            }

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Voice, -AccessKey, -SecretKey, or -Region.");
            }
        }
    }
}
