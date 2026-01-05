using System.Management.Automation;
using Voice.Cmdlets.Recognition;

namespace Voice.Cmdlets.Recognition
{
    /// <summary>
    /// Start background voice recognition
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "VoiceListening")]
    internal class StartVoiceListeningCmdlet : PSCmdlet
    {
        [Parameter]
        public string Culture { get; set; } = "en-US";

        protected override void ProcessRecord()
        {
            if (VoiceRecognitionState.IsListening)
            {
                WriteWarning("Voice recognition is already running");
                return;
            }

            try
            {
                VoiceRecognitionState.StartListening(Culture);
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
