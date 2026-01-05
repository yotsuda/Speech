using System;

namespace Voice.Cmdlets.Common
{
    /// <summary>
    /// Utility class for detecting backchannel responses (acknowledgments, filler words)
    /// </summary>
    public static class BackchannelDetector
    {
        /// <summary>
        /// Maximum duration in seconds for a backchannel utterance
        /// </summary>
        public const double MaxBackchannelDuration = 0.8;

        /// <summary>
        /// Common backchannel patterns in multiple languages
        /// </summary>
        private static readonly string[] BackchannelPatterns = new[]
        {
            // Japanese
            "うん", "ええ", "へー", "ほー", "なるほど", "そうですか", "はい",

            // English
            "uh-huh", "yeah", "okay", "right", "i see", "mm-hmm", "oh", "yes", "yep", "yup"
        };

        /// <summary>
        /// Determines if the given text and duration represent a backchannel response
        /// </summary>
        /// <param name="text">The recognized text</param>
        /// <param name="duration">Duration of the utterance in seconds</param>
        /// <returns>True if this is likely a backchannel response</returns>
        public static bool IsBackchannel(string text, double duration)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Utterances longer than the threshold are not backchannels
            if (duration > MaxBackchannelDuration)
                return false;

            var normalized = text.Trim().ToLower();

            // Check against known patterns
            foreach (var pattern in BackchannelPatterns)
            {
                if (normalized.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(pattern + " ", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the given text matches backchannel patterns (duration-independent)
        /// </summary>
        /// <param name="text">The recognized text</param>
        /// <returns>True if the text matches a backchannel pattern</returns>
        public static bool MatchesBackchannelPattern(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var normalized = text.Trim().ToLower();

            foreach (var pattern in BackchannelPatterns)
            {
                if (normalized.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
