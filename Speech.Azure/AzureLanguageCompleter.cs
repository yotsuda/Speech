using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using Speech.Core;

namespace Speech.Azure
{
    /// <summary>
    /// Provides argument completion for Azure language/locale codes.
    /// </summary>
    public class AzureLanguageCompleter : IArgumentCompleter
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
                // Check if Azure credentials are configured
                var config = ConfigManager.GetConfig();
                if (string.IsNullOrEmpty(config.Azure?.Key) || string.IsNullOrEmpty(config.Azure?.Region))
                {
                    results.Add(new CompletionResult("⚠ Run: Set-SpeechConfig -AzureKey <key> -AzureRegion <region>", "⚠ Run: Set-SpeechConfig -AzureKey <key> -AzureRegion <region>",
                        CompletionResultType.Text,
                        "Azure credentials not configured. Run Set-SpeechConfig first."));
                    return results;
                }

                var voices = AzureSpeechCompleter.GetCachedVoicesPublic();
                if (voices == null || voices.Count == 0)
                {
                    results.Add(new CompletionResult("⚠ Failed to fetch voice list from Azure", "⚠ Failed to fetch voice list from Azure",
                        CompletionResultType.Text,
                        "Could not retrieve voices. Check your Azure credentials."));
                    return results;
                }

                // Get distinct locales from API
                var locales = new HashSet<string>();
                foreach (var v in voices)
                {
                    if (!string.IsNullOrEmpty(v.Locale))
                        locales.Add(v.Locale);
                }

                var sortedLocales = new List<string>(locales);
                sortedLocales.Sort();

                foreach (var locale in sortedLocales)
                {
                    if (string.IsNullOrEmpty(wordToComplete) ||
                        locale.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        int count = 0;
                        foreach (var v in voices)
                        {
                            if (v.Locale == locale) count++;
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
