using System;
using System.Management.Automation;
using System.Text;
using NAudio.Wave;

namespace Speech.Core
{
    [Cmdlet(VerbsDiagnostic.Test, "Microphone")]
    public class TestMicrophoneCmdlet : PSCmdlet
    {
        [Parameter()]
        [ValidateRange(1, 60)]
        public int TestDurationSeconds { get; set; } = 5;

        [Parameter()]
        [ArgumentCompleter(typeof(MicrophoneCompleter))]
        public string? Microphone { get; set; }

        protected override void ProcessRecord()
        {
            var originalEncoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                var config = ConfigManager.GetConfig();
                var micName = Microphone ?? config.Common?.Microphone;
                var deviceNumber = MicrophoneHelper.GetDeviceNumber(micName);

                // Show device info
                var deviceName = deviceNumber >= 0
                    ? WaveInEvent.GetCapabilities(deviceNumber).ProductName
                    : WaveInEvent.GetCapabilities(0).ProductName;

                Console.WriteLine("Starting microphone test...");
                Console.WriteLine($"Device: {deviceName}");
                Console.WriteLine($"Test duration: {TestDurationSeconds} seconds");
                Console.WriteLine("Please speak something.");
                Console.WriteLine();

                bool audioDetected = false;
                double maxLevel = 0;
                double avgLevel = 0;
                int levelCount = 0;
                double currentLevel = 0;
                var lockObj = new object();

                using var waveIn = new WaveInEvent
                {
                    DeviceNumber = deviceNumber >= 0 ? deviceNumber : 0,
                    WaveFormat = new WaveFormat(16000, 16, 1)
                };

                waveIn.DataAvailable += (sender, e) =>
                {
                    // Calculate peak level from samples
                    double peak = 0;
                    for (int i = 0; i < e.BytesRecorded; i += 2)
                    {
                        var sample = Math.Abs((double)BitConverter.ToInt16(e.Buffer, i));
                        if (sample > peak) peak = sample;
                    }

                    // Normalize to 0-100 range (16-bit audio max = 32768)
                    var level = peak / 32768.0 * 100.0;

                    lock (lockObj)
                    {
                        currentLevel = level;
                        if (level > 1.0)
                        {
                            audioDetected = true;
                            maxLevel = Math.Max(maxLevel, level);
                            avgLevel = (avgLevel * levelCount + level) / (levelCount + 1);
                            levelCount++;
                        }
                    }
                };

                waveIn.StartRecording();

                var endTime = DateTime.Now.AddSeconds(TestDurationSeconds);
                int lastLineLength = 0;

                while (DateTime.Now < endTime)
                {
                    System.Threading.Thread.Sleep(100);

                    double level;
                    lock (lockObj)
                    {
                        level = currentLevel;
                    }

                    int barLength = (int)(level / 2);
                    string bar = new string('\u2588', Math.Min(barLength, 50));
                    string barPadding = new string(' ', 50 - bar.Length);
                    string line = $"  Level: [{bar}{barPadding}] {level,5:F1}";

                    string clearPadding = lastLineLength > line.Length
                        ? new string(' ', lastLineLength - line.Length)
                        : "";

                    Console.Write($"\r{line}{clearPadding}");
                    lastLineLength = line.Length;
                }

                waveIn.StopRecording();

                Console.WriteLine();
                Console.WriteLine();

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
