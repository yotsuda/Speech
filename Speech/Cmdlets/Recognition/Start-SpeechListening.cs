using System.Management.Automation;
using Speech.Cmdlets.Recognition;

namespace Speech.Cmdlets.Recognition
{
    /// <summary>
    /// Start background voice recognition
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "SpeechListening")]
    internal class StartVoiceListeningCmdlet : PSCmdlet
    {
        [Parameter]
        public string Culture { get; set; } = "en-US";

        protected override void ProcessRecord()
        {
            if (SpeechRecognitionState.IsListening)
            {
                WriteWarning("Voice recognition is already running");
                return;
            }

            try
            {
                SpeechRecognitionState.StartListening(Culture);
                WriteVerbose($"Background voice recognition started (Culture: {Culture})");
            }
            catch (System.Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "StartListeningError",
                    ErrorCategory.InvalidOperation,
                    null));
            }
        }
    }
}
