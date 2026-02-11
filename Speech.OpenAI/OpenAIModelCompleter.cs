using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Speech.OpenAI
{
    /// <summary>
    /// Provides argument completion for OpenAI speech models.
    /// Shows TTS models for Out-OpenAISpeech, STT models for Read-OpenAISpeech.
    /// </summary>
    public class OpenAIModelCompleter : IArgumentCompleter
    {
        private static readonly (string Name, string Description)[] TTSModels = new[]
        {
            ("tts-1", "Standard TTS - fast, lower quality"),
            ("tts-1-hd", "HD TTS - slower, higher quality"),
            ("gpt-4o-mini-tts", "GPT-4o mini TTS - newest, expressive"),
        };

        private static readonly (string Name, string Description)[] STTModels = new[]
        {
            ("whisper-1", "Whisper v2 - general-purpose speech recognition"),
        };

        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();

            var models = commandName.Contains("Read", StringComparison.OrdinalIgnoreCase)
                || parameterName.Contains("STT", StringComparison.OrdinalIgnoreCase)
                ? STTModels
                : TTSModels;

            foreach (var (name, description) in models)
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
