using System.Management.Automation;
using Speech.Cmdlets.Recognition;

namespace Speech.Cmdlets.Recognition
{
    /// <summary>
    /// Stop background voice recognition
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "SpeechListening")]
    internal class StopVoiceListeningCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            if (!SpeechRecognitionState.IsListening)
            {
                WriteWarning("Voice recognition is not running");
                return;
            }

            try
            {
                SpeechRecognitionState.StopListening();
                WriteVerbose("Background voice recognition stopped");
            }
            catch (System.Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "StopListeningError",
                    ErrorCategory.InvalidOperation,
                    null));
            }
        }
    }
}
