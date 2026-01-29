using System;
using System.Management.Automation;
using Speech.Core;

namespace Speech.Azure
{
    /// <summary>
    /// Base class for Azure Speech Services cmdlets
    /// </summary>
    public abstract class AzureCmdlet : PSCmdlet
    {
        [Parameter()]
        public string? Key { get; set; }

        [Parameter()]
        public string? Region { get; set; }

        protected bool _keySpecified;
        protected bool _regionSpecified;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            // Check which parameters were explicitly specified
            _keySpecified = MyInvocation.BoundParameters.ContainsKey(nameof(Key));
            _regionSpecified = MyInvocation.BoundParameters.ContainsKey(nameof(Region));

            // Load from config if not specified
            var config = ConfigManager.GetConfig();

            if (!_keySpecified && config.Azure?.Key != null)
            {
                Key = config.Azure.Key;
            }

            if (!_regionSpecified && config.Azure?.Region != null)
            {
                Region = config.Azure.Region;
            }

            // Validate credentials
            ValidateAzureCredentials();
        }

        /// <summary>
        /// Validates Azure credentials
        /// </summary>
        protected void ValidateAzureCredentials()
        {
            if (string.IsNullOrEmpty(Key))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException(
                        "Azure Speech API key is required. Specify with -Key parameter or save configuration:\n" +
                        "  Out-AzureVoice 'text' -Key 'your-key' -Region 'japaneast'\n" +
                        "Available regions: japaneast, eastus, westeurope, etc.\n" +
                        "Get your key at: https://portal.azure.com"),
                    "MissingAzureKey",
                    ErrorCategory.InvalidArgument,
                    null));
            }

            if (string.IsNullOrEmpty(Region))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException(
                        "Azure Speech Region is required. Specify with -Region parameter or save configuration:\n" +
                        "  Out-AzureVoice 'text' -Key 'your-key' -Region 'japaneast'\n" +
                        "Common regions: japaneast, eastus, westeurope, southeastasia, etc.\n" +
                        "See: https://aka.ms/speech/sdkregion"),
                    "MissingAzureRegion",
                    ErrorCategory.InvalidArgument,
                    null));
            }
        }
    }
}