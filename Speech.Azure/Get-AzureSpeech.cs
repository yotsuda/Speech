using System.Management.Automation;

namespace Speech.Azure
{
    [Cmdlet(VerbsCommon.Get, "AzureSpeech")]
    public class GetAzureVoiceCmdlet : AzureCmdlet
    {
        [Parameter()]
        public string? Locale { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var task = GetVoicesAsync();
                var voices = task.GetAwaiter().GetResult();

                foreach (var voice in voices)
                {
                    WriteObject(voice);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "AzureGetVoicesError", ErrorCategory.InvalidOperation, null));
            }
        }

        private async Task<System.Collections.Generic.List<AzureSpeechInfo>> GetVoicesAsync()
        {
            var manager = AzureAudioManager.GetInstance(Key!, Region);
            var allVoices = await manager.GetAvailableVoicesAsync();
            var filtered = new System.Collections.Generic.List<AzureSpeechInfo>();

            foreach (var voice in allVoices)
            {
                // Filter by locale if specified
                if (!string.IsNullOrEmpty(Locale) && !voice.Locale.StartsWith(Locale, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                filtered.Add(voice);
            }

            return filtered;
        }
    }
}
