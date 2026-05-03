using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using Speech.Core;

namespace Speech.Amazon
{
    /// <summary>
    /// Provides argument completion for Amazon Polly language/locale codes.
    /// </summary>
    public class AmazonLanguageCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();

            try
            {
                var config = ConfigManager.GetConfig();
                if (string.IsNullOrEmpty(config.Amazon?.AccessKey) ||
                    string.IsNullOrEmpty(config.Amazon?.SecretKey) ||
                    string.IsNullOrEmpty(config.Amazon?.Region))
                {
                    results.Add(new CompletionResult(
                        "⚠ Run: Set-AmazonSpeechConfig -AccessKey <key> -SecretKey <key> -Region <region>",
                        "⚠ Run: Set-AmazonSpeechConfig -AccessKey <key> -SecretKey <key> -Region <region>",
                        CompletionResultType.Text,
                        "Amazon credentials not configured. Run Set-AmazonSpeechConfig first."));
                    return results;
                }

                var voices = AmazonSpeechCompleter.GetCachedVoicesPublic();
                if (voices == null || voices.Count == 0)
                {
                    results.Add(new CompletionResult(
                        "⚠ Failed to fetch voice list from Amazon Polly",
                        "⚠ Failed to fetch voice list from Amazon Polly",
                        CompletionResultType.Text,
                        "Could not retrieve voices. Check your Amazon credentials."));
                    return results;
                }

                // Get distinct language codes from voice list
                var languages = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var v in voices)
                {
                    if (!string.IsNullOrEmpty(v.LanguageCode))
                    {
                        if (languages.ContainsKey(v.LanguageCode))
                            languages[v.LanguageCode]++;
                        else
                            languages[v.LanguageCode] = 1;
                    }
                }

                var sortedLanguages = new List<string>(languages.Keys);
                sortedLanguages.Sort(StringComparer.OrdinalIgnoreCase);

                foreach (var lang in sortedLanguages)
                {
                    if (string.IsNullOrEmpty(wordToComplete) ||
                        lang.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        var tooltip = $"{lang} ({languages[lang]} voices)";

                        results.Add(new CompletionResult(
                            lang,
                            lang,
                            CompletionResultType.ParameterValue,
                            tooltip));
                    }
                }
            }
            catch
            {
                // Silently fail - completion is best-effort
            }

            return results;
        }
    }
}
