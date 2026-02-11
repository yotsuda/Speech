using System;
using System.Management.Automation;
using System.Text;

namespace Speech.Core
{
    [Cmdlet(VerbsCommon.Get, "SpeechConfig")]
    [OutputType(typeof(string))]
    public class GetSpeechConfigCmdlet : PSCmdlet
    {
        [Parameter]
        public SwitchParameter Path { get; set; }

        protected override void ProcessRecord()
        {
            var configPath = ConfigManager.GetConfigFilePath();

            // If -Path is specified, just return the path
            if (Path)
            {
                WriteObject(configPath);
                return;
            }

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

            // Check which modules are installed
            var windowsInstalled = IsModuleInstalled("Speech.Windows");
            var azureInstalled = IsModuleInstalled("Speech.Azure");
            var openAIInstalled = IsModuleInstalled("Speech.OpenAI");
            var googleInstalled = IsModuleInstalled("Speech.Google");

            var sb = new StringBuilder();

            // Common settings (always shown)
            sb.AppendLine("[Common]");
            sb.AppendLine($"  Rate       : {config.Common?.Rate?.ToString("F2") ?? "(not set)"}");
            sb.AppendLine($"  Volume     : {config.Common?.Volume?.ToString() ?? "(not set)"}");
            sb.AppendLine($"  Language   : {config.Common?.Language ?? "(not set)"}");
            sb.AppendLine($"  Microphone : {config.Common?.Microphone ?? "(not set)"}");
            sb.AppendLine($"  OutputDevice : {(string.IsNullOrEmpty(config.Common?.OutputDevice) ? "(not set)" : config.Common.OutputDevice)}");

            // Windows settings (only if Speech.Windows is installed)
            if (windowsInstalled)
            {
                sb.AppendLine();
                sb.AppendLine("[Windows]");
                sb.AppendLine($"  Voice      : {config.Windows?.Voice ?? "(not set)"}");
            }

            // Azure settings (only if Speech.Azure is installed)
            if (azureInstalled)
            {
                sb.AppendLine();
                sb.AppendLine("[Azure]");
                sb.AppendLine($"  Voice      : {config.Azure?.Voice ?? "(not set)"}");
                sb.AppendLine($"  Pitch      : {config.Azure?.Pitch?.ToString() ?? "(not set)"}");
                sb.AppendLine($"  Region     : {config.Azure?.Region ?? "(not set)"}");
                sb.AppendLine($"  Key        : {MaskKey(config.Azure?.Key)}");
            }

            // OpenAI settings (only if Speech.OpenAI is installed)
            if (openAIInstalled)
            {
                sb.AppendLine();
                sb.AppendLine("[OpenAI]");
                sb.AppendLine($"  Voice      : {config.OpenAI?.Voice ?? "(not set)"}");
                sb.AppendLine($"  Model      : {config.OpenAI?.Model ?? "(not set)"}");
                sb.AppendLine($"  STTModel   : {config.OpenAI?.STTModel ?? "(not set)"}");
                sb.AppendLine($"  Key        : {MaskKey(config.OpenAI?.Key)}");
            }

            // Google settings (only if Speech.Google is installed)
            if (googleInstalled)
            {
                sb.AppendLine();
                sb.AppendLine("[Google]");
                sb.AppendLine($"  Credential : {config.Google?.Credential ?? "(not set)"}");
                sb.AppendLine($"  Voice      : {config.Google?.Voice ?? "(not set)"}");
            }

            WriteObject(sb.ToString());
        }

        private bool IsModuleInstalled(string moduleName)
        {
            try
            {
                using var ps = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
                ps.AddCommand("Get-Module")
                  .AddParameter("Name", moduleName)
                  .AddParameter("ListAvailable");
                var result = ps.Invoke();
                return result.Count > 0;
            }
            catch
            {
                return false;
            }
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
