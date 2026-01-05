using System;
using System.Collections.Concurrent;
using System.Speech.Recognition;
using Voice.Cmdlets.Windows;

namespace Voice.Cmdlets.Recognition
{
    /// <summary>
    /// Static class to manage background voice recognition
    /// </summary>
    internal static class VoiceRecognitionState
    {
        private static SpeechRecognitionEngine? _recognizer;
        private static readonly ConcurrentQueue<VoiceInputResult> _inputQueue = new();
        private static bool _isListening = false;
        private static readonly object _lock = new();
        private static string _currentCulture = "en-US";

        public static bool IsListening => _isListening;

        public static void StartListening(string culture = "en-US")
        {
            lock (_lock)
            {
                if (_isListening)
                {
                    return; // Already listening
                }

                _currentCulture = culture;
                _recognizer = WindowsAudioManager.GetRecognizer(culture);

                _recognizer.SpeechRecognized += OnSpeechRecognized;

                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                _isListening = true;
            }
        }

        public static void StopListening()
        {
            lock (_lock)
            {
                if (!_isListening || _recognizer == null)
                {
                    return;
                }

                _recognizer.RecognizeAsyncStop();
                _recognizer.SpeechRecognized -= OnSpeechRecognized;
                _recognizer = null;
                _isListening = false;

                // Clear queue
                while (_inputQueue.TryDequeue(out _)) { }
            }
        }

        public static VoiceInputResult? GetInput()
        {
            if (_inputQueue.TryDequeue(out var result))
            {
                return result;
            }
            return null;
        }

        public static VoiceInputResult? WaitForInput(int timeoutSeconds)
        {
            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromSeconds(timeoutSeconds);

            while ((DateTime.Now - startTime) < timeout)
            {
                if (_inputQueue.TryDequeue(out var result))
                {
                    return result;
                }
                System.Threading.Thread.Sleep(100);
            }

            return null; // Timeout
        }

        private static void OnSpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                var result = new VoiceInputResult
                {
                    Text = e.Result.Text,
                    Confidence = e.Result.Confidence,
                    Duration = e.Result.Audio.Duration.TotalSeconds,
                    Timestamp = DateTime.Now
                };

                _inputQueue.Enqueue(result);
            }
            catch
            {
                // Ignore errors in event handler
            }
        }
    }

    internal class VoiceInputResult
    {
        public string Text { get; set; } = "";
        public float Confidence { get; set; }
        public double Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
