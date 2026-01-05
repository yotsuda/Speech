using System.Management.Automation;
using Voice.Cmdlets.Recognition;

namespace Voice.Cmdlets.Recognition
{
    /// <summary>
    /// Stop background voice recognition
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "VoiceListening")]
    internal class StopVoiceListeningCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            if (!VoiceRecognitionState.IsListening)
            {
                WriteWarning("Voice recognition is not running");
                return;
            }

            try
            {
                VoiceRecognitionState.StopListening();
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
