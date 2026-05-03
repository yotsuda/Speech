using System;
using System.Management.Automation;
using Speech.Core;

namespace Speech.Amazon
{
    [Cmdlet(VerbsCommon.Get, "AmazonSpeech")]
    [OutputType(typeof(PSObject))]
    public class GetAmazonSpeechCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(AmazonLanguageCompleter))]
        public string? Language { get; set; }

        [Parameter]
        public string? AccessKey { get; set; }

        [Parameter]
        public string? SecretKey { get; set; }

        [Parameter]
        public string? Region { get; set; }

        protected override void ProcessRecord()
        {
            var config = ConfigManager.GetConfig();

            var accessKey = AccessKey ?? config.Amazon?.AccessKey;
            var secretKey = SecretKey ?? config.Amazon?.SecretKey;
            var region = Region ?? config.Amazon?.Region;

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException(
                        "Amazon credentials not specified. Use -AccessKey/-SecretKey or Set-AmazonSpeechConfig -AccessKey <key> -SecretKey <key>"),
                    "NoCredential",
                    ErrorCategory.InvalidOperation,
                    null));
                return;
            }

            if (string.IsNullOrEmpty(region))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException(
                        "Amazon region not specified. Use -Region or Set-AmazonSpeechConfig -Region <region>"),
                    "NoRegion",
                    ErrorCategory.InvalidOperation,
                    null));
                return;
            }

            try
            {
                using var manager = new AmazonAudioManager(accessKey, secretKey, region);
                var voices = manager.GetVoicesAsync(Language).GetAwaiter().GetResult();

                foreach (var voice in voices)
                {
                    var obj = new PSObject();
                    obj.TypeNames.Insert(0, "Speech.Amazon.AmazonSpeechInfo");
                    obj.Properties.Add(new PSNoteProperty("Id", voice.Id));
                    obj.Properties.Add(new PSNoteProperty("Name", voice.Name));
                    obj.Properties.Add(new PSNoteProperty("Gender", voice.Gender));
                    obj.Properties.Add(new PSNoteProperty("Language", voice.LanguageCode));
                    obj.Properties.Add(new PSNoteProperty("LanguageName", voice.LanguageName));
                    obj.Properties.Add(new PSNoteProperty("Engines", string.Join(", ", voice.SupportedEngines)));
                    WriteObject(obj);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "AmazonVoicesError", ErrorCategory.InvalidOperation, null));
            }
        }
    }
}
