using System.Runtime.InteropServices;
using System.Text;
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
        public string Culture { get; set; } = System.Globalization.CultureInfo.CurrentCulture.Name;

        protected override void ProcessRecord()
        {
            // Ensure UTF-8 output for emoji support
            var originalEncoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                Console.WriteLine("Starting microphone test...");
                Console.WriteLine($"Test duration: {TestDurationSeconds} seconds");
                Console.WriteLine("Please speak something.");
                Console.WriteLine();

                var recognizer = WindowsAudioManager.GetRecognizer(Culture);

                // Audio level detection variables
                bool audioDetected = false;
                double maxLevel = 0;
                double avgLevel = 0;
                int levelCount = 0;
                double currentLevel = 0;
                object lockObj = new object();

                // Audio level event handler (only update variables, no WriteProgress)
                EventHandler<AudioLevelUpdatedEventArgs> levelHandler = (sender, e) =>
                {
                    lock (lockObj)
                    {
                        currentLevel = e.AudioLevel;
                        if (e.AudioLevel > 0)
                        {
                            audioDetected = true;
                            maxLevel = Math.Max(maxLevel, e.AudioLevel);
                            avgLevel = (avgLevel * levelCount + e.AudioLevel) / (levelCount + 1);
                            levelCount++;
                        }
                    }
                };

                recognizer.AudioLevelUpdated += levelHandler;

                try
                {
                    recognizer.RecognizeAsync(RecognizeMode.Multiple);

                    // Display progress on main thread
                    var endTime = DateTime.Now.AddSeconds(TestDurationSeconds);
                    int lastLineLength = 0;

                    while (DateTime.Now < endTime)
                    {
                        Thread.Sleep(100);

                        double level;
                        lock (lockObj)
                        {
                            level = currentLevel;
                        }

                        // Display audio level bar
                        int barLength = (int)(level / 2);
                        string bar = new string('\u2588', Math.Min(barLength, 50)); // █
                        string barPadding = new string(' ', 50 - bar.Length);
                        string line = $"  Level: [{bar}{barPadding}] {level,5:F1}";
                        
                        // Clear any leftover characters
                        string clearPadding = lastLineLength > line.Length 
                            ? new string(' ', lastLineLength - line.Length) 
                            : "";
                        
                        Console.Write($"\r{line}{clearPadding}");
                        lastLineLength = line.Length;
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    // Display test results
                    Console.WriteLine("=== Microphone Test Results ===");
                    Console.WriteLine($"Audio detected: {(audioDetected ? "\u2705 Yes" : "\u274C No")}");
                    
                    if (audioDetected)
                    {
                        Console.WriteLine($"Max level: {maxLevel:F1}");
                        Console.WriteLine($"Avg level: {avgLevel:F1}");
                        Console.WriteLine($"Sample count: {levelCount}");
                        Console.WriteLine();
                        
                        if (maxLevel > 30)
                        {
                            Console.WriteLine("\u2705 Microphone is working properly.");
                        }
                        else if (maxLevel > 10)
                        {
                            Console.WriteLine("\u26A0 Microphone volume seems low.");
                        }
                        else
                        {
                            Console.WriteLine("\u274C Microphone volume is very low or not working properly.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\u274C No audio was detected.");
                        Console.WriteLine("  - Check if microphone is connected");
                        Console.WriteLine("  - Check microphone volume settings");
                        Console.WriteLine("  - Check if other apps are using the microphone");
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
            finally
            {
                Console.OutputEncoding = originalEncoding;
            }
        }
    }
}