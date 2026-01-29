using System;
using System.Management.Automation;

namespace Speech.Core
{
    [Cmdlet(VerbsCommon.Get, "SpeechConfig")]
    public class GetSpeechConfigCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var configPath = ConfigManager.GetConfigFilePath();

            if (!ConfigManager.ConfigExists())
            {
                WriteWarning($"No configuration file found at: {configPath}");
                return;
            }

            var config = ConfigManager.GetConfig();

            if (config == null)
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException($"Failed to load configuration from: {configPath}"),
                    "ConfigLoadError",
                    ErrorCategory.InvalidOperation,
                    configPath));
                return;
            }

            // Output Common settings
            if (config.Common != null)
            {
                var common = new PSObject();
                common.TypeNames.Insert(0, "Voice.Config.Common");
                common.Properties.Add(new PSNoteProperty("Section", "Common"));
                common.Properties.Add(new PSNoteProperty("Rate", config.Common.Rate));
                common.Properties.Add(new PSNoteProperty("Volume", config.Common.Volume));
                common.Properties.Add(new PSNoteProperty("Microphone", config.Common.Microphone));
                WriteObject(common);
            }

            // Output Windows settings
            if (config.Windows != null)
            {
                var windows = new PSObject();
                windows.TypeNames.Insert(0, "Voice.Config.Windows");
                windows.Properties.Add(new PSNoteProperty("Section", "Windows"));
                windows.Properties.Add(new PSNoteProperty("Speech", config.Windows.Voice));
                WriteObject(windows);
            }

            // Output Azure settings
            if (config.Azure != null)
            {
                var azure = new PSObject();
                azure.TypeNames.Insert(0, "Voice.Config.Azure");
                azure.Properties.Add(new PSNoteProperty("Section", "Azure"));
                azure.Properties.Add(new PSNoteProperty("Speech", config.Azure.Voice));
                azure.Properties.Add(new PSNoteProperty("Pitch", config.Azure.Pitch));
                azure.Properties.Add(new PSNoteProperty("Region", config.Azure.Region));

                // Mask API key for security (show only last 4 chars)
                string? maskedKey = null;
                if (!string.IsNullOrEmpty(config.Azure.Key))
                {
                    maskedKey = config.Azure.Key.Length > 4
                        ? new string('*', config.Azure.Key.Length - 4) + config.Azure.Key.Substring(config.Azure.Key.Length - 4)
                        : new string('*', config.Azure.Key.Length);
                }
                azure.Properties.Add(new PSNoteProperty("Key", maskedKey));
                WriteObject(azure);
            }
        }
    }
}
