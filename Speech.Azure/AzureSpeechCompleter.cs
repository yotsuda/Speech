using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using Speech.Core;

namespace Speech.Azure
{
    /// <summary>
    /// Provides argument completion for Azure voice names.
    /// Caches voice list to avoid repeated API calls.
    /// Filters by Language/AzureLanguage parameter if specified.
    /// </summary>
    public class AzureSpeechCompleter : IArgumentCompleter
    {
        private static List<AzureSpeechInfo>? _cachedVoices;
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
                // Check if Azure credentials are configured
                var config = ConfigManager.GetConfig();
                if (string.IsNullOrEmpty(config.Azure?.Key) || string.IsNullOrEmpty(config.Azure?.Region))
                {
                    results.Add(new CompletionResult("⚠ Run: Set-SpeechConfig -AzureKey <key> -AzureRegion <region>", "⚠ Run: Set-SpeechConfig -AzureKey <key> -AzureRegion <region>",
                        CompletionResultType.Text,
                        "Azure credentials not configured. Run Set-SpeechConfig first."));
                    return results;
                }

                var voices = GetCachedVoices();
                if (voices == null || voices.Count == 0)
                {
                    results.Add(new CompletionResult("⚠ Failed to fetch voice list from Azure", "⚠ Failed to fetch voice list from Azure",
                        CompletionResultType.Text,
                        "Could not retrieve voices. Check your Azure credentials."));
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
                    // Priority 1: Use language from Azure Voice setting
                    var configVoice = config.Azure?.Voice;
                    if (!string.IsNullOrEmpty(configVoice))
                    {
                        languageFilter = ExtractLanguageFromVoice(configVoice);
                    }
                    // Priority 2: Use Common Language setting
                    else if (!string.IsNullOrEmpty(config.Common?.Language))
                    {
                        languageFilter = ConfigManager.NormalizeLanguage(config.Common.Language);
                    }
                }

                foreach (var voice in voices)
                {
                    var shortName = voice.ShortName ?? voice.Name ?? "";
                    if (string.IsNullOrEmpty(shortName))
                        continue;

                    // Filter by language (e.g., "ja", "ja-JP", "en")
                    if (!string.IsNullOrEmpty(languageFilter))
                    {
                        var locale = voice.Locale ?? "";
                        if (!locale.StartsWith(languageFilter, StringComparison.OrdinalIgnoreCase))
                            continue;
                    }

                    if (string.IsNullOrEmpty(wordToComplete) ||
                        shortName.Contains(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        var displayName = voice.LocalName ?? voice.DisplayName ?? shortName;
                        var tooltip = $"{shortName}  {voice.Locale}  {voice.Gender}";

                        results.Add(new CompletionResult(
                            $"'{shortName}'",
                            displayName,
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

        /// <summary>
        /// Gets voices from cache or fetches from API if cache is expired.
        /// </summary>
        private static List<AzureSpeechInfo>? GetCachedVoices()
        {
            lock (_cacheLock)
            {
                if (_cachedVoices != null && DateTime.Now - _cacheTime < CacheDuration)
                {
                    return _cachedVoices;
                }
            }

            // Get credentials from config
            var config = ConfigManager.GetConfig();
            var key = config.Azure?.Key;
            var region = config.Azure?.Region;

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(region))
                return null;

            try
            {
                var manager = AzureAudioManager.GetInstance(key, region);
                var task = manager.GetAvailableVoicesAsync();
                var voices = task.GetAwaiter().GetResult();

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
        /// Gets cached voices for use by other completers (e.g., AzureLanguageCompleter).
        /// </summary>
        public static List<AzureSpeechInfo>? GetCachedVoicesPublic() => GetCachedVoices();

        /// <summary>
        /// Clears the voice cache.
        /// </summary>
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _cachedVoices = null;
                _cacheTime = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Extracts language code from voice name (e.g., "ja-JP-NanamiNeural" -> "ja-JP")
        /// </summary>
        private static string? ExtractLanguageFromVoice(string voice)
        {
            if (string.IsNullOrEmpty(voice))
                return null;

            var parts = voice.Split('-');
            if (parts.Length >= 2)
            {
                return $"{parts[0]}-{parts[1]}";
            }

            return null;
        }
    }
}
