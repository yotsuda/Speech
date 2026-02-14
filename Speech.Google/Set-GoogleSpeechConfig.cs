using System.Management.Automation;
using Speech.Core;

namespace Speech.Google
{
    [Cmdlet(VerbsCommon.Set, "GoogleSpeechConfig")]
    public class SetGoogleSpeechConfigCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(GoogleSpeechCompleter))]
        public string? Voice { get; set; }

        [Parameter]
        public string? Credential { get; set; }

        protected override void ProcessRecord()
        {
            bool updated = false;

            if (!string.IsNullOrEmpty(Voice))
            {
                ConfigManager.UpdateGoogleVoiceIfSpecified(Voice);
                WriteVerbose($"Google voice set to: {Voice}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Credential))
            {
                var resolvedPath = GetUnresolvedProviderPathFromPSPath(Credential);
                if (!System.IO.File.Exists(resolvedPath))
                {
                    WriteWarning($"Credential file not found: {resolvedPath}");
                    WriteWarning("The path will be saved, but authentication will fail until the file exists.");
                }
                ConfigManager.UpdateGoogleCredentialIfSpecified(resolvedPath);
                WriteVerbose($"Google credential set to: {resolvedPath}");
                updated = true;
            }

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Voice or -Credential.");
            }
        }
    }
}
