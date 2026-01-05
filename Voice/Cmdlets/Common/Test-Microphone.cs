using System.Runtime.InteropServices;
using Voice.Cmdlets.Windows;
using System.Management.Automation;
using System.Speech.Recognition;

namespace Voice.Cmdlets.Common
{
    [Cmdlet(VerbsDiagnostic.Test, "Microphone")]
    public class TestMicrophoneCmdlet : WindowsCmdlet
    {
        [Parameter()]
        public int TestDurationSeconds { get; set; } = 5;

        [Parameter()]
        public string Culture { get; set; } = "en-US";

        protected override void ProcessRecord()
        {
            try
            {
                WriteObject("Starting microphone test...");
                WriteObject($"Test duration: {TestDurationSeconds} seconds");
                WriteObject("Please speak something.");

                var recognizer = WindowsAudioManager.GetRecognizer(Culture);

                // Audio level detection variables
                bool audioDetected = false;
                double maxLevel = 0;
                double avgLevel = 0;
                int levelCount = 0;

                // Audio level event handler
                EventHandler<AudioLevelUpdatedEventArgs> levelHandler = (sender, e) =>
                {
                    if (e.AudioLevel > 0)
                    {
                        audioDetected = true;
                        maxLevel = Math.Max(maxLevel, e.AudioLevel);
                        avgLevel = (avgLevel * levelCount + e.AudioLevel) / (levelCount + 1);
                        levelCount++;

                        // Show real-time audio level (simplified)
                        if (levelCount % 10 == 0) // Show every 10th reading
                        {
                            WriteProgress(new ProgressRecord(1, "Audio Level", 
                                $"Current: {e.AudioLevel:F2} Max: {maxLevel:F2} Avg: {avgLevel:F2}"));
                        }
                    }
                };

                recognizer.AudioLevelUpdated += levelHandler;

                try
                {
                    // Start recognition for audio level detection only
                    recognizer.RecognizeAsync(RecognizeMode.Multiple);

                    // Wait for specified duration
                    Thread.Sleep(TestDurationSeconds * 1000);

                    // Display test results
                    WriteObject("\n=== Microphone Test Results ===");
                    WriteObject($"Audio detected: {(audioDetected ? "✅ Success" : "❌ Failed")}");
                    
                    if (audioDetected)
                    {
                        WriteObject($"Maximum audio level: {maxLevel:F2}");
                        WriteObject($"Average audio level: {avgLevel:F2}");
                        WriteObject($"Audio detection count: {levelCount}");
                        
                        if (maxLevel > 30)
                        {
                            WriteObject("✅ Microphone is working properly.");
                        }
                        else if (maxLevel > 10)
                        {
                            WriteObject("⚠️ Microphone volume seems low.");
                        }
                        else
                        {
                            WriteObject("❌ Microphone volume is very low or not working properly.");
                        }
                    }
                    else
                    {
                        WriteObject("❌ No audio was detected.");
                        WriteObject("  - Check if microphone is connected");
                        WriteObject("  - Check microphone volume settings");
                        WriteObject("  - Check if other applications are using the microphone");
                    }
                }
                finally
                {
                    recognizer.RecognizeAsyncStop();
                    recognizer.AudioLevelUpdated -= levelHandler;
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "MicrophoneTestError", ErrorCategory.InvalidOperation, null));
            }
        }
    }
}



