using System.Text.Json;

namespace Speech.Cmdlets.Common
{
    /// <summary>
    /// Manages configuration persistence for Voice module with in-memory caching
    /// </summary>
    public static class ConfigManager
    {
        private static SpeechConfig? _cachedConfig = null;
        private static readonly object _cacheLock = new object();

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Gets the base path for Voice module data
        /// </summary>
        public static string GetBasePath()
        {
            string moduleName = "Speech";
            if (OperatingSystem.IsWindows())
            {
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(documents, "PowerShell", "Modules", moduleName);
            }
            else // Unix (Linux / macOS)
            {
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(home, ".local", "share", "powershell", "Modules", moduleName);
            }
        }

        /// <summary>
        /// Gets the configuration file path
        /// </summary>
        public static string GetConfigFilePath()
        {
            string configFileName = "SpeechConfig.json";
            return Path.Combine(GetBasePath(), configFileName);
        }

        /// <summary>
        /// Gets the configuration (from cache or loads/creates from file on first access)
        /// Automatically creates and saves default configuration if no file exists
        /// </summary>
        public static SpeechConfig GetConfig()
        {
            if (_cachedConfig == null)
            {
                lock (_cacheLock)
                {
                    if (_cachedConfig == null)
                    {
                        var loaded = LoadConfigFromFile();

                        if (loaded == null)
                        {
                            // Create default config and save it immediately
                            loaded = CreateDefaultConfig();
                            SaveConfigToFile(loaded);
                        }

                        _cachedConfig = loaded;
                    }
                }
            }
            return _cachedConfig;
        }

        /// <summary>
        /// Saves configuration to file and updates cache
        /// </summary>
        public static void SaveConfig(SpeechConfig config)
        {
            lock (_cacheLock)
            {
                SaveConfigToFile(config);
                _cachedConfig = config;
            }
        }

        /// <summary>
        /// Saves configuration to file (internal helper)
        /// </summary>
        private static void SaveConfigToFile(SpeechConfig config)
        {
            string configPath = GetConfigFilePath();
            string? directory = Path.GetDirectoryName(configPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(config, JsonOptions);
            File.WriteAllText(configPath, json);
        }

        /// <summary>
        /// Creates default configuration based on system locale
        /// </summary>
        private static SpeechConfig CreateDefaultConfig()
        {
            return new SpeechConfig
            {
                Common = new CommonConfig
                {
                    Rate = 1.0,
                    Volume = 100
                },
                Windows = new WindowsConfig
                {
                    Voice = GetDefaultWindowsVoice()
                },
                Azure = new AzureConfig
                {                    Voice = GetDefaultAzureVoice()
                }
            };
        }

        /// <summary>
        /// Gets default Windows voice based on system locale
        /// </summary>
        private static string? GetDefaultWindowsVoice()
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return null;
            }

            try
            {
                var synthesizer = global::Speech.Cmdlets.Windows.WindowsAudioManager.GetSynthesizer();
                var currentCulture = System.Globalization.CultureInfo.CurrentUICulture;
                var voices = synthesizer?.GetInstalledVoices();

                if (voices == null || voices.Count == 0)
                    return null;

                // 1. Exact culture match (e.g., ja-JP)
                var exactMatch = voices.FirstOrDefault(v =>
                    v.Enabled &&
                    v.VoiceInfo.Culture.Name.Equals(currentCulture.Name, StringComparison.OrdinalIgnoreCase));

                if (exactMatch != null)
                    return exactMatch.VoiceInfo.Name;

                // 2. Language match (e.g., ja)
                var languageMatch = voices.FirstOrDefault(v =>
                    v.Enabled &&
                    v.VoiceInfo.Culture.TwoLetterISOLanguageName == currentCulture.TwoLetterISOLanguageName);

                if (languageMatch != null)
                    return languageMatch.VoiceInfo.Name;

                // 3. English voice as fallback
                var englishVoice = voices.FirstOrDefault(v =>
                    v.Enabled &&
                    v.VoiceInfo.Culture.TwoLetterISOLanguageName == "en");

                if (englishVoice != null)
                    return englishVoice.VoiceInfo.Name;

                // 4. First available voice
                return voices.FirstOrDefault(v => v.Enabled)?.VoiceInfo.Name;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets default Azure voice based on system locale
        /// </summary>
        private static string GetDefaultAzureVoice()
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture;

            // TODO: Hardcoded for now. Consider fetching locale-appropriate voices via API.
            return culture.TwoLetterISOLanguageName switch
            {
                "ja" => "ja-JP-NanamiNeural",
                "zh" => "zh-CN-XiaoxiaoNeural",
                "ko" => "ko-KR-SunHiNeural",
                "de" => "de-DE-KatjaNeural",
                "fr" => "fr-FR-DeniseNeural",
                "es" => "es-ES-ElviraNeural",
                "it" => "it-IT-ElsaNeural",
                "pt" => "pt-BR-FranciscaNeural",
                "ru" => "ru-RU-SvetlanaNeural",
                _ => "en-US-JennyNeural"
            };
        }


        /// <summary>
        /// Loads configuration from file (internal use only)
        /// </summary>
        private static SpeechConfig? LoadConfigFromFile()
        {
            string configPath = GetConfigFilePath();

            if (!File.Exists(configPath))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<SpeechConfig>(json, JsonOptions);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes the configuration file and clears cache
        /// </summary>
        public static bool DeleteConfig()
        {
            lock (_cacheLock)
            {
                string configPath = GetConfigFilePath();

                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                    _cachedConfig = null;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if configuration file exists
        /// </summary>
        public static bool ConfigExists()
        {
            return File.Exists(GetConfigFilePath());
        }

        /// <summary>
        /// Gets Windows voice value from parameter or config
        /// </summary>
        public static string? GetWindowsVoice(string? parameterValue)
        {
            if (parameterValue != null)
                return parameterValue;

            var config = GetConfig();
            return config.Windows?.Voice;
        }

        /// <summary>
        /// Gets rate value from parameter or config with default
        /// </summary>
        public static double GetRate(double? parameterValue, double defaultValue = 1.0)
        {
            if (parameterValue.HasValue)
                return parameterValue.Value;

            var config = GetConfig();
            return config.Common?.Rate ?? defaultValue;
        }

        /// <summary>
        /// Gets volume value from parameter or config with default
        /// </summary>
        public static int GetVolume(int? parameterValue, int defaultValue = 100)
        {
            if (parameterValue.HasValue)
                return parameterValue.Value;

            var config = GetConfig();
            return config.Common?.Volume ?? defaultValue;
        }

        /// <summary>
        /// Updates Windows voice setting if parameter was specified
        /// </summary>
        public static bool UpdateWindowsVoiceIfSpecified(string? parameterValue)
        {
            if (parameterValue == null)
                return false;

            var config = GetConfig();
            if (config.Windows?.Voice == parameterValue)
                return false;

            config.Windows ??= new WindowsConfig();
            config.Windows.Voice = parameterValue;
            SaveConfig(config);
            return true;
        }

        /// <summary>
        /// Updates common rate setting if parameter was specified
        /// </summary>
        public static bool UpdateRateIfSpecified(double? parameterValue)
        {
            if (!parameterValue.HasValue)
                return false;

            var config = GetConfig();
            if (config.Common?.Rate == parameterValue.Value)
                return false;

            config.Common ??= new CommonConfig();
            config.Common.Rate = parameterValue.Value;
            SaveConfig(config);
            return true;
        }

        /// <summary>
        /// Updates common volume setting if parameter was specified
        /// </summary>
        public static bool UpdateVolumeIfSpecified(int? parameterValue)
        {
            if (!parameterValue.HasValue)
                return false;

            var config = GetConfig();
            if (config.Common?.Volume == parameterValue.Value)
                return false;

            config.Common ??= new CommonConfig();
            config.Common.Volume = parameterValue.Value;
            SaveConfig(config);
            return true;
        }

        /// <summary>
        /// Gets microphone value from parameter or config
        /// </summary>
        public static string? GetMicrophone(string? parameterValue)
        {
            if (parameterValue != null)
                return parameterValue;

            var config = GetConfig();
            return config.Common?.Microphone;
        }

        /// <summary>
        /// Updates microphone setting if parameter was specified
        /// </summary>
        public static bool UpdateMicrophoneIfSpecified(string? parameterValue)
        {
            if (parameterValue == null)
                return false;

            var config = GetConfig();
            if (config.Common?.Microphone == parameterValue)
                return false;

            config.Common ??= new CommonConfig();
            config.Common.Microphone = parameterValue;
            SaveConfig(config);
            return true;
        }

        /// <summary>
        /// Gets language value from parameter or config
        /// </summary>
        public static string? GetLanguage(string? parameterValue)
        {
            if (parameterValue != null)
                return parameterValue;

            var config = GetConfig();
            return config.Common?.Language;
        }

        /// <summary>
        /// Updates language setting and clears conflicting Voice settings.
        /// Returns list of cleared voice settings for warning purposes.
        /// </summary>
        public static List<string> UpdateLanguageIfSpecified(string? parameterValue)
        {
            var clearedVoices = new List<string>();

            if (parameterValue == null)
                return clearedVoices;

            var config = GetConfig();
            var normalizedLanguage = NormalizeLanguage(parameterValue);

            // Check if Azure Voice conflicts with new language
            if (!string.IsNullOrEmpty(config.Azure?.Voice))
            {
                var azureVoiceLanguage = ExtractLanguageFromVoice(config.Azure.Voice);
                if (azureVoiceLanguage != null &&
                    !azureVoiceLanguage.StartsWith(normalizedLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    clearedVoices.Add($"AzureVoice '{config.Azure.Voice}'");
                    config.Azure.Voice = null;
                }
            }

            // Update language
            config.Common ??= new CommonConfig();
            config.Common.Language = parameterValue;
            SaveConfig(config);

            return clearedVoices;
        }

        /// <summary>
        /// Normalizes a language code (e.g., "ja" -> "ja-JP", "en" -> "en-US")
        /// </summary>
        public static string NormalizeLanguage(string language)
        {
            // If already in full format (e.g., "ja-JP"), return as-is
            if (language.Contains('-'))
                return language;

            // Map short codes to full locale codes
            return language.ToLowerInvariant() switch
            {
                "ja" => "ja-JP",
                "en" => "en-US",
                "zh" => "zh-CN",
                "ko" => "ko-KR",
                "de" => "de-DE",
                "fr" => "fr-FR",
                "es" => "es-ES",
                "it" => "it-IT",
                "pt" => "pt-BR",
                "ru" => "ru-RU",
                _ => $"{language}-{language.ToUpperInvariant()}"  // Guess: "xx" -> "xx-XX"
            };
        }

        /// <summary>
        /// Extracts language code from voice name (e.g., "ja-JP-NanamiNeural" -> "ja-JP")
        /// </summary>
        public static string? ExtractLanguageFromVoice(string? voice)
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

        /// <summary>
        /// Updates Azure key setting if parameter was specified
        /// </summary>
        public static bool UpdateAzureKeyIfSpecified(string? parameterValue)
        {
            if (parameterValue == null)
                return false;

            var config = GetConfig();
            if (config.Azure?.Key == parameterValue)
                return false;

            config.Azure ??= new AzureConfig();
            config.Azure.Key = parameterValue;
            SaveConfig(config);
            return true;
        }

        /// <summary>
        /// Updates Azure region setting if parameter was specified
        /// </summary>
        public static bool UpdateAzureRegionIfSpecified(string? parameterValue)
        {
            if (parameterValue == null)
                return false;

            var config = GetConfig();
            if (config.Azure?.Region == parameterValue)
                return false;

            config.Azure ??= new AzureConfig();
            config.Azure.Region = parameterValue;
            SaveConfig(config);
            return true;
        }

        /// <summary>
        /// Updates Azure voice setting if parameter was specified
        /// </summary>
        public static bool UpdateAzureVoiceIfSpecified(string? parameterValue)
        {
            if (parameterValue == null)
                return false;

            var config = GetConfig();
            if (config.Azure?.Voice == parameterValue)
                return false;

            config.Azure ??= new AzureConfig();
            config.Azure.Voice = parameterValue;
            SaveConfig(config);
            return true;
        }

        /// <summary>
        /// Updates Azure pitch setting if parameter was specified
        /// </summary>
        public static bool UpdateAzurePitchIfSpecified(int? parameterValue)
        {
            if (!parameterValue.HasValue)
                return false;

            var config = GetConfig();
            if (config.Azure?.Pitch == parameterValue.Value)
                return false;

            config.Azure ??= new AzureConfig();
            config.Azure.Pitch = parameterValue.Value;
            SaveConfig(config);
            return true;
        }
    }

    /// <summary>
    /// Configuration structure for Voice
    /// </summary>
    public class SpeechConfig
    {
        public CommonConfig? Common { get; set; }
        public WindowsConfig? Windows { get; set; }
        public AzureConfig? Azure { get; set; }
    }

    public class CommonConfig
    {
        public double? Rate { get; set; }
        public int? Volume { get; set; }
        public string? Microphone { get; set; }
        public string? Language { get; set; }
    }

    public class WindowsConfig
    {
        public string? Voice { get; set; }
    }

    public class AzureConfig
    {
        public string? Voice { get; set; }
        public int? Pitch { get; set; }
        public string? Region { get; set; }
        public string? Key { get; set; }
    }
}


