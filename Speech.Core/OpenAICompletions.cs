using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Speech.Core
{
    /// <summary>
    /// Provides argument completion for OpenAI TTS voice names.
    /// Used by Set-SpeechConfig in Speech.Core where Speech.OpenAI types are not available.
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
            string commandName, string parameterName, string wordToComplete,
            CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();
            foreach (var (name, description) in Voices)
            {
                if (string.IsNullOrEmpty(wordToComplete) ||
                    name.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new CompletionResult(name, name,
                        CompletionResultType.ParameterValue, description));
                }
            }
            return results;
        }
    }

    /// <summary>
    /// Provides argument completion for OpenAI TTS model names.
    /// Used by Set-SpeechConfig in Speech.Core.
    /// </summary>
    public class OpenAITTSModelCompleter : IArgumentCompleter
    {
        private static readonly (string Name, string Description)[] Models = new[]
        {
            ("tts-1", "Standard TTS - fast, lower quality"),
            ("tts-1-hd", "HD TTS - slower, higher quality"),
            ("gpt-4o-mini-tts", "GPT-4o mini TTS - newest, expressive"),
        };

        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName, string parameterName, string wordToComplete,
            CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();
            foreach (var (name, description) in Models)
            {
                if (string.IsNullOrEmpty(wordToComplete) ||
                    name.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new CompletionResult(name, name,
                        CompletionResultType.ParameterValue, description));
                }
            }
            return results;
        }
    }

    /// <summary>
    /// Provides argument completion for OpenAI STT model names.
    /// Used by Set-SpeechConfig in Speech.Core.
    /// </summary>
    public class OpenAISTTModelCompleter : IArgumentCompleter
    {
        private static readonly (string Name, string Description)[] Models = new[]
        {
            ("whisper-1", "Whisper v2 - general-purpose speech recognition"),
        };

        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName, string parameterName, string wordToComplete,
            CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            var results = new List<CompletionResult>();
            foreach (var (name, description) in Models)
            {
                if (string.IsNullOrEmpty(wordToComplete) ||
                    name.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new CompletionResult(name, name,
                        CompletionResultType.ParameterValue, description));
                }
            }
            return results;
        }
    }
}
