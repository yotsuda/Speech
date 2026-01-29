using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Voice.Cmdlets.Azure
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
                var voices = AzureVoiceCompleter.GetCachedVoicesPublic();
                if (voices == null)
                    return results;

                // Get distinct locales
                var locales = voices
                    .Where(v => !string.IsNullOrEmpty(v.Locale))
                    .Select(v => v.Locale!)
                    .Distinct()
                    .OrderBy(l => l);

                foreach (var locale in locales)
                {
                    if (string.IsNullOrEmpty(wordToComplete) ||
                        locale.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        // Count voices for this locale
                        var count = voices.Count(v => v.Locale == locale);
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
