using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Voice.Cmdlets.Azure
{
    [Cmdlet(VerbsLifecycle.Invoke, "AzureVoiceRecognition")]
    public class InvokeAzureVoiceRecognitionCmdlet : AzureCmdlet
    {
        [Parameter()]
        public string Language { get; set; } = "en-US";

        [Parameter()]
        public int TimeoutSeconds { get; set; } = 30;

        [Parameter()]
        public SwitchParameter Continuous { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Continuous.IsPresent)
                {
                    var task = RecognizeContinuousAsync();
                    task.GetAwaiter().GetResult();
                }
                else
                {
                    var task = RecognizeOnceAsync();
                    task.GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "AzureSpeechRecognitionError", ErrorCategory.InvalidOperation, null));
            }
        }

        private async Task RecognizeOnceAsync()
        {
            var manager = AzureAudioManager.GetInstance(Key!, Region);
            
            WriteVerbose($"Starting speech recognition (Language: {Language}, Timeout: {TimeoutSeconds}s)");
            Host.UI.WriteLine("Speak now...");

            var result = await manager.RecognizeOnceAsync(Language, TimeoutSeconds);

            if (!string.IsNullOrEmpty(result))
            {
                WriteObject(result);
            }
            else
            {
                WriteWarning("No speech recognized or recognition timed out.");
            }
        }

        private async Task RecognizeContinuousAsync()
        {
            var manager = AzureAudioManager.GetInstance(Key!, Region);
            
            WriteVerbose($"Starting continuous speech recognition (Language: {Language})");
            Host.UI.WriteLine("Speak continuously. Press Enter to stop...");

            var recognizedTexts = await manager.RecognizeContinuousAsync(Language, () => {
                // Check if Enter key is pressed
                return Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter;
            });

            foreach (var text in recognizedTexts)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    WriteObject(text);
                }
            }

            if (recognizedTexts.Count == 0)
            {
                WriteWarning("No speech recognized.");
            }
        }
    }
}

