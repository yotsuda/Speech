using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Speech.Core;

namespace Speech.Google
{
    [Cmdlet(VerbsCommon.Get, "GoogleSpeech")]
    [OutputType(typeof(PSObject))]
    public class GetGoogleSpeechCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(GoogleLanguageCompleter))]
        public string? Language { get; set; }

        [Parameter]
        public string? Credential { get; set; }

        protected override void ProcessRecord()
        {
            var config = ConfigManager.GetConfig();

            var credentialPath = Credential ?? config.Google?.Credential;
            if (string.IsNullOrEmpty(credentialPath))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException("Google credential path not specified. Use -Credential or Set-SpeechConfig -GoogleCredential"),
                    "NoCredential",
                    ErrorCategory.InvalidOperation,
                    null));
                return;
            }

            if (!File.Exists(credentialPath))
            {
                ThrowTerminatingError(new ErrorRecord(
                    new FileNotFoundException($"Credential file not found: {credentialPath}"),
                    "CredentialNotFound",
                    ErrorCategory.ObjectNotFound,
                    credentialPath));
                return;
            }

            try
            {
                using var manager = new GoogleAudioManager(credentialPath);
                var voices = manager.GetVoicesAsync(Language).GetAwaiter().GetResult();

                foreach (var voice in voices)
                {
                    var obj = new PSObject();
                    obj.TypeNames.Insert(0, "Speech.Google.GoogleSpeechInfo");
                    obj.Properties.Add(new PSNoteProperty("Name", voice.Name));
                    obj.Properties.Add(new PSNoteProperty("Gender", voice.Gender));
                    obj.Properties.Add(new PSNoteProperty("Languages", string.Join(", ", voice.LanguageCodes)));
                    WriteObject(obj);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GoogleVoicesError", ErrorCategory.InvalidOperation, null));
            }
        }
    }
}
