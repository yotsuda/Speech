using System.Management.Automation;
using Speech.Cmdlets.Recognition;
using Speech.Cmdlets.Common;

namespace Speech.Cmdlets.Recognition
{
    /// <summary>
    /// Get voice input from background recognition
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SpeechInput")]
    [OutputType(typeof(string), typeof(VoiceInputResult))]
    internal class GetVoiceInputCmdlet : PSCmdlet
    {
        [Parameter]
        public int WaitSeconds { get; set; } = 0;

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        [Parameter]
        public SwitchParameter IgnoreBackchannel { get; set; }

        protected override void ProcessRecord()
        {
            if (!SpeechRecognitionState.IsListening)
            {
                WriteWarning("Voice recognition is not running. Use Start-VoiceListening first.");
                WriteObject(null);
                return;
            }

            VoiceInputResult? result = null;

            if (WaitSeconds == 0)
            {
                // Poll only (non-blocking)
                result = SpeechRecognitionState.GetInput();
            }
            else
            {
                // Wait for input (blocking)
                WriteVerbose($"Waiting for voice input (timeout: {WaitSeconds}s)");
                result = SpeechRecognitionState.WaitForInput(WaitSeconds);
            }

            // Check for backchannel
            if (result != null && IgnoreBackchannel)
            {
                if (BackchannelDetector.IsBackchannel(result.Text, result.Duration))
                {
                    WriteVerbose($"Backchannel detected, ignoring: {result.Text}");
                    WriteObject(null);
                    return;
                }
            }

            if (result == null)
            {
                if (WaitSeconds > 0)
                {
                    WriteVerbose("Voice input timeout");
                }
                WriteObject(null);
                return;
            }

            WriteVerbose($"Voice input received: {result.Text} (Confidence: {result.Confidence:F2})");

            if (PassThru)
            {
                var output = new PSObject();
                output.Properties.Add(new PSNoteProperty("Text", result.Text));
                output.Properties.Add(new PSNoteProperty("Confidence", result.Confidence));
                output.Properties.Add(new PSNoteProperty("Duration", result.Duration));
                output.Properties.Add(new PSNoteProperty("Timestamp", result.Timestamp));
                WriteObject(output);
            }
            else
            {
                WriteObject(result.Text);
            }
        }
    }
}
