using System;

namespace Speech.Core
{
    /// <summary>
    /// Provides animated spinner display for speech recognition feedback.
    /// </summary>
    public class SpinnerDisplay
    {
        private static readonly string[] SpinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
        private int _spinnerIndex = 0;
        private int _lastDisplayLength = 0;

        /// <summary>
        /// Displays the current text with an animated spinner.
        /// </summary>
        public void DisplaySpinner(string text)
        {
            var spinner = SpinnerFrames[_spinnerIndex];
            _spinnerIndex = (_spinnerIndex + 1) % SpinnerFrames.Length;

            string display;
            if (string.IsNullOrEmpty(text))
            {
                display = $"> {spinner}";
            }
            else
            {
                display = $"> {text} {spinner}";
            }
            var padding = _lastDisplayLength > display.Length
                ? new string(' ', _lastDisplayLength - display.Length)
                : "";

            Console.Write($"\r{display}{padding}");
            _lastDisplayLength = display.Length;
        }

        /// <summary>
        /// Displays the final recognized text and moves to a new line.
        /// </summary>
        /// <param name="text">The recognized text.</param>
        /// <param name="showNextPrompt">If true, shows "> " prompt for next input.</param>
        public void DisplayFinal(string text, bool showNextPrompt = false)
        {
            var display = $"> {text}";
            var padding = _lastDisplayLength > display.Length
                ? new string(' ', _lastDisplayLength - display.Length)
                : "";

            Console.WriteLine($"\r{display}{padding}");

            if (showNextPrompt)
            {
                Console.Write("> ");
                _lastDisplayLength = 2;
            }
            else
            {
                _lastDisplayLength = 0;
            }
        }

        /// <summary>
        /// Clears the current line.
        /// </summary>
        public void ClearCurrentLine()
        {
            if (_lastDisplayLength > 0)
            {
                Console.Write($"\r{new string(' ', _lastDisplayLength)}\r");
            }
        }
    }
}
