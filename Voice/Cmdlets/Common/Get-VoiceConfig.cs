using System;
using System.Management.Automation;
using Voice.Cmdlets.Common;

namespace Voice.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Get, "VoiceConfig")]

    public class GetVoiceConfigCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var configPath = ConfigManager.GetConfigFilePath();

            if (!ConfigManager.ConfigExists())
            {
                WriteWarning($"No configuration file found at: {configPath}");
                WriteHost("Use Out-WindowsVoice with parameters to create a configuration.");
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

            WriteHost($"\nConfiguration loaded from: {configPath}\n", ConsoleColor.Cyan);

            // Display Common settings
            if (config.Common != null)
            {
                WriteHost("Common:", ConsoleColor.Green);
                if (config.Common.Rate != null)
                    WriteHost($"  Rate       : {config.Common.Rate}", ConsoleColor.White);
                if (config.Common.Volume != null)
                    WriteHost($"  Volume     : {config.Common.Volume}", ConsoleColor.White);
                if (!string.IsNullOrEmpty(config.Common.Microphone))
                    WriteHost($"  Microphone : {config.Common.Microphone}", ConsoleColor.White);
                WriteHost("");
            }

            // Display Windows settings
            if (config.Windows != null)
            {
                WriteHost("Windows:", ConsoleColor.Green);
                if (!string.IsNullOrEmpty(config.Windows.Voice))
                    WriteHost($"  Voice  : {config.Windows.Voice}", ConsoleColor.White);
                WriteHost("");
            }

            // Display Azure settings
            if (config.Azure != null)
            {
                bool hasAzureSettings = !string.IsNullOrEmpty(config.Azure.Voice)
                    || !string.IsNullOrEmpty(config.Azure.Region)
                    || !string.IsNullOrEmpty(config.Azure.Key)
                    || config.Azure.Pitch != null;

                if (hasAzureSettings)
                {
                    WriteHost("Azure:", ConsoleColor.Green);
                    if (!string.IsNullOrEmpty(config.Azure.Voice))
                        WriteHost($"  Voice  : {config.Azure.Voice}", ConsoleColor.White);
                    if (config.Azure.Pitch != null)
                        WriteHost($"  Pitch  : {config.Azure.Pitch}", ConsoleColor.White);
                    if (!string.IsNullOrEmpty(config.Azure.Region))
                        WriteHost($"  Region : {config.Azure.Region}", ConsoleColor.White);
                    if (!string.IsNullOrEmpty(config.Azure.Key))
                    {
                        // Mask API key for security (show only last 4 chars)
                        var maskedKey = config.Azure.Key.Length > 4
                            ? new string('*', config.Azure.Key.Length - 4) + config.Azure.Key.Substring(config.Azure.Key.Length - 4)
                            : new string('*', config.Azure.Key.Length);
                        WriteHost($"  Key    : {maskedKey}", ConsoleColor.White);
                    }
                    WriteHost("");
                }
            }
        }

        private void WriteHost(string message, ConsoleColor? foregroundColor = null)
        {
            var hostUI = Host?.UI;
            if (hostUI != null)
            {
                if (foregroundColor.HasValue)
                {
                    var oldColor = hostUI.RawUI.ForegroundColor;
                    hostUI.RawUI.ForegroundColor = foregroundColor.Value;
                    hostUI.WriteLine(message);
                    hostUI.RawUI.ForegroundColor = oldColor;
                }
                else
                {
                    hostUI.WriteLine(message);
                }
            }
        }
    }
}
