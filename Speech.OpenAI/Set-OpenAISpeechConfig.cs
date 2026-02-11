using System.Management.Automation;
using Speech.Core;

namespace Speech.OpenAI
{
    [Cmdlet(VerbsCommon.Set, "OpenAISpeechConfig")]
    public class SetOpenAISpeechConfigCmdlet : PSCmdlet
    {
        [Parameter]
        [ArgumentCompleter(typeof(OpenAIVoiceCompleter))]
        public string? Voice { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(OpenAIModelCompleter))]
        public string? Model { get; set; }

        [Parameter]
        [ArgumentCompleter(typeof(OpenAIModelCompleter))]
        public string? STTModel { get; set; }

        [Parameter]
        public string? Key { get; set; }

        protected override void ProcessRecord()
        {
            bool updated = false;

            if (!string.IsNullOrEmpty(Voice))
            {
                ConfigManager.UpdateOpenAIVoiceIfSpecified(Voice);
                WriteVerbose($"OpenAI voice set to: {Voice}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Model))
            {
                ConfigManager.UpdateOpenAIModelIfSpecified(Model);
                WriteVerbose($"OpenAI TTS model set to: {Model}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(STTModel))
            {
                ConfigManager.UpdateOpenAISTTModelIfSpecified(STTModel);
                WriteVerbose($"OpenAI STT model set to: {STTModel}");
                updated = true;
            }

            if (!string.IsNullOrEmpty(Key))
            {
                ConfigManager.UpdateOpenAIKeyIfSpecified(Key);
                WriteVerbose("OpenAI key updated");
                updated = true;
            }

            if (updated)
            {
                WriteObject($"Configuration saved to: {ConfigManager.GetConfigFilePath()}");
            }
            else
            {
                WriteWarning("No parameters specified. Use -Voice, -Model, -STTModel, or -Key.");
            }
        }
    }
}
