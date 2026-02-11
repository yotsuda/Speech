using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Speech.OpenAI
{
    /// <summary>
    /// Provides argument completion for OpenAI Whisper supported languages.
    /// </summary>
    public class OpenAILanguageCompleter : IArgumentCompleter
    {
        private static readonly (string Code, string Name)[] Languages = new[]
        {
            ("af", "Afrikaans"),
            ("ar", "Arabic"),
            ("bg", "Bulgarian"),
            ("bn", "Bengali"),
            ("ca", "Catalan"),
            ("cs", "Czech"),
            ("da", "Danish"),
            ("de", "German"),
            ("el", "Greek"),
            ("en", "English"),
            ("es", "Spanish"),
            ("et", "Estonian"),
            ("fi", "Finnish"),
            ("fr", "French"),
            ("gl", "Galician"),
            ("gu", "Gujarati"),
            ("he", "Hebrew"),
            ("hi", "Hindi"),
            ("hr", "Croatian"),
            ("hu", "Hungarian"),
            ("id", "Indonesian"),
            ("it", "Italian"),
            ("ja", "Japanese"),
            ("kn", "Kannada"),
            ("ko", "Korean"),
            ("lt", "Lithuanian"),
            ("lv", "Latvian"),
            ("mk", "Macedonian"),
            ("ml", "Malayalam"),
            ("mr", "Marathi"),
            ("ms", "Malay"),
            ("nl", "Dutch"),
            ("no", "Norwegian"),
            ("pa", "Punjabi"),
            ("pl", "Polish"),
            ("pt", "Portuguese"),
            ("ro", "Romanian"),
            ("ru", "Russian"),
            ("sk", "Slovak"),
            ("sl", "Slovenian"),
            ("sr", "Serbian"),
            ("sv", "Swedish"),
            ("sw", "Swahili"),
            ("ta", "Tamil"),
            ("te", "Telugu"),
            ("th", "Thai"),
            ("tl", "Tagalog"),
            ("tr", "Turkish"),
            ("uk", "Ukrainian"),
            ("ur", "Urdu"),
            ("vi", "Vietnamese"),
            ("zh", "Chinese"),
        };

        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();

            foreach (var (code, name) in Languages)
            {
                if (string.IsNullOrEmpty(wordToComplete) ||
                    code.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new CompletionResult(
                        code,
                        $"{code} - {name}",
                        CompletionResultType.ParameterValue,
                        name));
                }
            }

            return results;
        }
    }
}
