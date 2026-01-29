using System.Runtime.InteropServices;
using System.Globalization;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Speech.Windows
{
    /// <summary>
    /// Manages shared Windows Speech API instances
    /// </summary>
    public static class WindowsAudioManager
    {
        private static SpeechSynthesizer? _synthesizer;
        private static readonly object _synthesizerLock = new();

        private static SpeechRecognitionEngine? _recognizer;
        private static readonly object _recognizerLock = new();
        private static string? _currentRecognizerCulture;

        /// <summary>
        /// Gets or creates a shared SpeechSynthesizer instance
        /// </summary>
        public static SpeechSynthesizer GetSynthesizer()
        {
            if (_synthesizer == null)
            {
                lock (_synthesizerLock)
                {
                    if (_synthesizer == null)
                    {
                        _synthesizer = new SpeechSynthesizer();
                        _synthesizer.SetOutputToDefaultAudioDevice();
                    }
                }
            }
            return _synthesizer;
        }

        /// <summary>
        /// Gets or creates a shared SpeechRecognitionEngine instance for the specified culture
        /// </summary>
        public static SpeechRecognitionEngine GetRecognizer(string culture = "en-US")
        {
            if (_recognizer == null || _currentRecognizerCulture != culture)
            {
                lock (_recognizerLock)
                {
                    if (_recognizer == null || _currentRecognizerCulture != culture)
                    {
                        // Dispose previous recognizer if culture changed
                        _recognizer?.Dispose();

                        var cultureInfo = new CultureInfo(culture);
                        _recognizer = new SpeechRecognitionEngine(cultureInfo);
                        _recognizer.LoadGrammar(new DictationGrammar());
                        _recognizer.SetInputToDefaultAudioDevice();
                        _currentRecognizerCulture = culture;
                    }
                }
            }
            return _recognizer;
        }


        /// <summary>
        /// Check if the current platform is Windows
        /// </summary>
        public static bool IsWindowsPlatform()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        /// <summary>
        /// Cleanup all resources
        /// </summary>
        public static void Cleanup()
        {
            lock (_synthesizerLock)
            {
                _synthesizer?.Dispose();
                _synthesizer = null;
            }

            lock (_recognizerLock)
            {
                _recognizer?.Dispose();
                _recognizer = null;
                _currentRecognizerCulture = null;
            }
        }
    }
}



