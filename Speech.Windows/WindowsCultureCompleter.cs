using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Speech.Windows
{
    /// <summary>
    /// Provides argument completion for Windows speech cultures.
    /// Lists distinct cultures from installed TTS voices.
    /// </summary>
    public class WindowsCultureCompleter : IArgumentCompleter
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
                var synthesizer = WindowsAudioManager.GetSynthesizer();
                var voices = synthesizer?.GetInstalledVoices();

                if (voices == null) return results;

                var cultures = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var voice in voices.Where(v => v.Enabled))
                {
                    var cultureName = voice.VoiceInfo.Culture.Name;
                    if (!string.IsNullOrEmpty(cultureName))
                    {
                        if (cultures.ContainsKey(cultureName))
                            cultures[cultureName]++;
                        else
                            cultures[cultureName] = 1;
                    }
                }

                foreach (var kvp in cultures.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(wordToComplete) ||
                        kvp.Key.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        var tooltip = $"{kvp.Key} ({kvp.Value} voices)";
                        results.Add(new CompletionResult(
                            kvp.Key,
                            kvp.Key,
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
