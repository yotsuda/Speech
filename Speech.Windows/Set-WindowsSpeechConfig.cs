using System.Management.Automation;
using Speech.Core;

namespace Speech.Windows
{
    [Cmdlet(VerbsCommon.Set, "WindowsSpeechConfig")]
    public class SetWindowsSpeechConfigCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(OutWindowsVoiceCmdlet.VoiceCompleter))]
        public string? Voice { get; set; }

        protected override void ProcessRecord()
        {
            bool updated = false;

            if (!string.IsNullOrEmpty(Voice))
            {
                ConfigManager.UpdateWindowsVoiceIfSpecified(Voice);
                WriteVerbose($"Windows voice set to: {Voice}");
                updated = true;
            }

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Voice.");
            }
        }
    }
}
