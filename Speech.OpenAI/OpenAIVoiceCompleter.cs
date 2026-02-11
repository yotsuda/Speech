using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Speech.OpenAI
{
    /// <summary>
    /// Provides argument completion for OpenAI TTS voice names.
    /// </summary>
    public class OpenAIVoiceCompleter : IArgumentCompleter
    {
        private static readonly (string Name, string Description)[] Voices = new[]
        {
            ("alloy", "Neutral, balanced"),
            ("ash", "Clear, confident"),
            ("ballad", "Warm, expressive"),
            ("coral", "Warm, friendly"),
            ("echo", "Smooth, authoritative"),
            ("fable", "Expressive, storytelling"),
            ("onyx", "Deep, resonant"),
            ("nova", "Energetic, bright"),
            ("sage", "Calm, thoughtful"),
            ("shimmer", "Clear, optimistic"),
            ("verse", "Versatile, dynamic"),
        };

        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();

            foreach (var (name, description) in Voices)
            {
                if (string.IsNullOrEmpty(wordToComplete) ||
                    name.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new CompletionResult(
                        name,
                        name,
                        CompletionResultType.ParameterValue,
                        description));
                }
            }

            return results;
        }
    }
}
