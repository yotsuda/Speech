using System;
using System.Management.Automation;
using System.Text;

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

            var sb = new StringBuilder();

            // Common settings
            sb.AppendLine("[Common]");
            sb.AppendLine($"  Rate       : {config.Common?.Rate?.ToString("F2") ?? "(not set)"}");
            sb.AppendLine($"  Volume     : {config.Common?.Volume?.ToString() ?? "(not set)"}");
            sb.AppendLine($"  Language   : {config.Common?.Language ?? "(not set)"}");
            sb.AppendLine($"  Microphone : {config.Common?.Microphone ?? "(not set)"}");
            sb.AppendLine();

            // Windows settings
            sb.AppendLine("[Windows]");
            sb.AppendLine($"  Voice      : {config.Windows?.Voice ?? "(not set)"}");
            sb.AppendLine();

            // Azure settings
            sb.AppendLine("[Azure]");
            sb.AppendLine($"  Voice      : {config.Azure?.Voice ?? "(not set)"}");
            sb.AppendLine($"  Pitch      : {config.Azure?.Pitch?.ToString() ?? "(not set)"}");
            sb.AppendLine($"  Region     : {config.Azure?.Region ?? "(not set)"}");
            sb.AppendLine($"  Key        : {MaskKey(config.Azure?.Key)}");
            sb.AppendLine();

            // OpenAI settings
            sb.AppendLine("[OpenAI]");
            sb.AppendLine($"  Voice      : {config.OpenAI?.Voice ?? "(not set)"}");
            sb.AppendLine($"  Model      : {config.OpenAI?.Model ?? "(not set)"}");
            sb.AppendLine($"  Key        : {MaskKey(config.OpenAI?.Key)}");

            WriteObject(sb.ToString());
        }

        private static string MaskKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
                return "(not set)";

            if (key.Length <= 8)
                return new string('*', key.Length);

            return new string('*', key.Length - 4) + key.Substring(key.Length - 4);
        }
    }
}
