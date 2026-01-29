using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using Voice.Cmdlets.Common;

namespace Voice.Cmdlets.Azure
{
    /// <summary>
    /// Provides argument completion for Azure voice names.
    /// Caches voice list to avoid repeated API calls.
    /// Filters by Language/AzureLanguage parameter if specified.
    /// </summary>
    public class AzureVoiceCompleter : IArgumentCompleter
    {
        private static List<AzureVoiceInfo>? _cachedVoices;
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
                var voices = GetCachedVoices();
                if (voices == null)
                    return results;

                // Get language filter from bound parameters
                // Out-AzureVoice uses -Language, Set-VoiceConfig uses -AzureLanguage
                string? languageFilter = null;
                if (fakeBoundParameters.Contains("Language"))
                {
                    languageFilter = fakeBoundParameters["Language"]?.ToString();
                }
                else if (fakeBoundParameters.Contains("AzureLanguage"))
                {
                    languageFilter = fakeBoundParameters["AzureLanguage"]?.ToString();
                }

                foreach (var voice in voices)
                {
                    var shortName = voice.ShortName ?? voice.Name ?? "";
                    if (string.IsNullOrEmpty(shortName))
                        continue;

                    // Filter by language if specified (e.g., "ja", "ja-JP", "en")
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
        private static List<AzureVoiceInfo>? GetCachedVoices()
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
        public static List<AzureVoiceInfo>? GetCachedVoicesPublic() => GetCachedVoices();

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
    }
}
