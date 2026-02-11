using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using Speech.Core;

namespace Speech.Google
{
    /// <summary>
    /// Provides argument completion for Google language/locale codes.
    /// </summary>
    public class GoogleLanguageCompleter : IArgumentCompleter
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
                if (string.IsNullOrEmpty(config.Google?.Credential))
                {
                    results.Add(new CompletionResult(
                        "⚠ Run: Set-SpeechConfig -GoogleCredential <path>",
                        "⚠ Run: Set-SpeechConfig -GoogleCredential <path>",
                        CompletionResultType.Text,
                        "Google credential not configured. Run Set-SpeechConfig first."));
                    return results;
                }

                var voices = GoogleSpeechCompleter.GetCachedVoicesPublic();
                if (voices == null || voices.Count == 0)
                {
                    results.Add(new CompletionResult(
                        "⚠ Failed to fetch voice list from Google",
                        "⚠ Failed to fetch voice list from Google",
                        CompletionResultType.Text,
                        "Could not retrieve voices. Check your Google credentials."));
                    return results;
                }

                // Get distinct locales from voice list
                var locales = new HashSet<string>();
                foreach (var v in voices)
                {
                    foreach (var lang in v.LanguageCodes)
                    {
                        if (!string.IsNullOrEmpty(lang))
                            locales.Add(lang);
                    }
                }

                var sortedLocales = new List<string>(locales);
                sortedLocales.Sort(StringComparer.OrdinalIgnoreCase);

                foreach (var locale in sortedLocales)
                {
                    if (string.IsNullOrEmpty(wordToComplete) ||
                        locale.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        int count = 0;
                        foreach (var v in voices)
                        {
                            foreach (var lang in v.LanguageCodes)
                            {
                                if (lang == locale) { count++; break; }
                            }
                        }
                        var tooltip = $"{locale} ({count} voices)";

                        results.Add(new CompletionResult(
                            locale,
                            locale,
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
