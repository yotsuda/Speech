using System;
using System.Management.Automation;
using Voice.Cmdlets.Common;

namespace Voice.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Get, "VoiceConfig")]
    [OutputType(typeof(PSObject))]
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
                    WriteHost($"  Rate   : {config.Common.Rate}", ConsoleColor.White);
                if (config.Common.Volume != null)
                    WriteHost($"  Volume : {config.Common.Volume}", ConsoleColor.White);
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
            
            // Display Azure settings (placeholder)
            if (config.Azure != null && !string.IsNullOrEmpty(config.Azure.Voice))
            {
                WriteHost("Azure:", ConsoleColor.Green);
                WriteHost($"  Voice  : {config.Azure.Voice}", ConsoleColor.White);
                if (config.Azure.Pitch != null)
                    WriteHost($"  Pitch  : {config.Azure.Pitch}", ConsoleColor.White);
                if (!string.IsNullOrEmpty(config.Azure.Region))
                    WriteHost($"  Region : {config.Azure.Region}", ConsoleColor.White);
                WriteHost("");
            }
            
            // Output as object for pipeline
            var result = new PSObject();
            result.Properties.Add(new PSNoteProperty("ConfigPath", configPath));
            result.Properties.Add(new PSNoteProperty("Common", config.Common));
            result.Properties.Add(new PSNoteProperty("Windows", config.Windows));
            result.Properties.Add(new PSNoteProperty("Azure", config.Azure));
            
            WriteObject(result);
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
