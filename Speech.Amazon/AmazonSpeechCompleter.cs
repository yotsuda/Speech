using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using Speech.Core;

namespace Speech.Amazon
{
    /// <summary>
    /// Provides argument completion for Amazon Polly voice names.
    /// Caches voice list to avoid repeated API calls.
    /// </summary>
    public class AmazonSpeechCompleter : IArgumentCompleter
    {
        private static List<AmazonVoiceInfo>? _cachedVoices;
        private static DateTime _cacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
        private static readonly object _cacheLock = new();

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
                var accessKey = config.Amazon?.AccessKey;
                var secretKey = config.Amazon?.SecretKey;
                var region = config.Amazon?.Region;

                if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(region))
                {
                    results.Add(new CompletionResult(
                        "⚠ Run: Set-AmazonSpeechConfig -AccessKey <key> -SecretKey <key> -Region <region>",
                        "⚠ Run: Set-AmazonSpeechConfig -AccessKey <key> -SecretKey <key> -Region <region>",
                        CompletionResultType.Text,
                        "Amazon credentials not configured. Run Set-AmazonSpeechConfig first."));
                    return results;
                }

                var voices = GetCachedVoices(accessKey, secretKey, region);
                if (voices == null || voices.Count == 0)
                {
                    results.Add(new CompletionResult(
                        "⚠ Failed to fetch voice list from Amazon Polly",
                        "⚠ Failed to fetch voice list from Amazon Polly",
                        CompletionResultType.Text,
                        "Could not retrieve voices. Check your Amazon credentials."));
                    return results;
                }

                // Get language filter from bound parameters (-Language)
                string? languageFilter = null;
                if (fakeBoundParameters.Contains("Language"))
                {
                    languageFilter = fakeBoundParameters["Language"]?.ToString();
                }

                // If no explicit language specified, check config
                if (string.IsNullOrEmpty(languageFilter))
                {
                    var configVoice = config.Amazon?.Voice;
                    if (!string.IsNullOrEmpty(configVoice))
                    {
                        // Find language from voice
                        var voiceInfo = voices.Find(v => v.Id.Equals(configVoice, StringComparison.OrdinalIgnoreCase));
                        if (voiceInfo != null)
                            languageFilter = voiceInfo.LanguageCode;
                    }
                    else if (!string.IsNullOrEmpty(config.Common?.Language))
                    {
                        languageFilter = ConfigManager.NormalizeLanguage(config.Common.Language);
                    }
                }

                foreach (var voice in voices)
                {
                    // Filter by language
                    if (!string.IsNullOrEmpty(languageFilter) &&
                        !voice.LanguageCode.StartsWith(languageFilter, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var id = voice.Id;
                    if (string.IsNullOrEmpty(wordToComplete) ||
                        id.Contains(wordToComplete, StringComparison.OrdinalIgnoreCase) ||
                        voice.Name.Contains(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        var engines = string.Join("/", voice.SupportedEngines);
                        var tooltip = $"{id} ({voice.Name})  {voice.LanguageCode}  {voice.Gender}  [{engines}]";

                        results.Add(new CompletionResult(
                            id,
                            id,
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

        private static List<AmazonVoiceInfo>? GetCachedVoices(string accessKey, string secretKey, string region)
        {
            lock (_cacheLock)
            {
                if (_cachedVoices != null && DateTime.Now - _cacheTime < CacheDuration)
                {
                    return _cachedVoices;
                }
            }

            try
            {
                using var manager = new AmazonAudioManager(accessKey, secretKey, region);
                var voices = manager.GetVoicesAsync().GetAwaiter().GetResult();

                lock (_cacheLock)
                {
                    _cachedVoices = voices;
                    _cacheTime = DateTime.Now;
                }

                return voices;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets cached voices for use by other completers (e.g., AmazonLanguageCompleter).
        /// </summary>
        public static List<AmazonVoiceInfo>? GetCachedVoicesPublic()
        {
            var config = ConfigManager.GetConfig();
            var accessKey = config.Amazon?.AccessKey;
            var secretKey = config.Amazon?.SecretKey;
            var region = config.Amazon?.Region;
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(region))
                return null;
            return GetCachedVoices(accessKey, secretKey, region);
        }
    }
}
