using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using Speech.Core;

namespace Speech.Google
{
    /// <summary>
    /// Provides argument completion for Google voice names.
    /// Caches voice list to avoid repeated API calls.
    /// Filters by Language parameter or config setting.
    /// </summary>
    public class GoogleSpeechCompleter : IArgumentCompleter
    {
        private static List<GoogleVoiceInfo>? _cachedVoices;
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
                var credentialPath = config.Google?.Credential;
                if (string.IsNullOrEmpty(credentialPath))
                {
                    results.Add(new CompletionResult(
                        "⚠ Run: Set-SpeechConfig -GoogleCredential <path>",
                        "⚠ Run: Set-SpeechConfig -GoogleCredential <path>",
                        CompletionResultType.Text,
                        "Google credential not configured. Run Set-SpeechConfig first."));
                    return results;
                }

                var voices = GetCachedVoices(credentialPath);
                if (voices == null || voices.Count == 0)
                {
                    results.Add(new CompletionResult(
                        "⚠ Failed to fetch voice list from Google",
                        "⚠ Failed to fetch voice list from Google",
                        CompletionResultType.Text,
                        "Could not retrieve voices. Check your Google credentials."));
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
                    // Priority 1: Use language from Google Voice setting
                    var configVoice = config.Google?.Voice;
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
                    var name = voice.Name;
                    if (string.IsNullOrEmpty(name))
                        continue;

                    // Filter by language
                    if (!string.IsNullOrEmpty(languageFilter))
                    {
                        bool matches = false;
                        foreach (var lang in voice.LanguageCodes)
                        {
                            if (lang.StartsWith(languageFilter, StringComparison.OrdinalIgnoreCase))
                            {
                                matches = true;
                                break;
                            }
                        }
                        if (!matches) continue;
                    }

                    if (string.IsNullOrEmpty(wordToComplete) ||
                        name.Contains(wordToComplete, StringComparison.OrdinalIgnoreCase))
                    {
                        var langs = string.Join(", ", voice.LanguageCodes);
                        var tooltip = $"{name}  {langs}  {voice.Gender}";

                        results.Add(new CompletionResult(
                            $"'{name}'",
                            name,
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

        private static List<GoogleVoiceInfo>? GetCachedVoices(string credentialPath)
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
                using var manager = new GoogleAudioManager(credentialPath);
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
        /// Gets cached voices for use by other completers (e.g., GoogleLanguageCompleter).
        /// </summary>
        public static List<GoogleVoiceInfo>? GetCachedVoicesPublic()
        {
            var config = ConfigManager.GetConfig();
            var credentialPath = config.Google?.Credential;
            if (string.IsNullOrEmpty(credentialPath))
                return null;
            return GetCachedVoices(credentialPath);
        }

        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _cachedVoices = null;
                _cacheTime = DateTime.MinValue;
            }
        }

        private static string? ExtractLanguageFromVoice(string voice)
        {
            if (string.IsNullOrEmpty(voice))
                return null;

            // Voice format: "ja-JP-Standard-A" -> "ja-JP"
            var parts = voice.Split('-');
            if (parts.Length >= 2)
            {
                return $"{parts[0]}-{parts[1]}";
            }

            return null;
        }
    }
}
