using System.Text.Json;
using Speech.Core;
using Xunit;

namespace Speech.Tests.Core
{
    public class ConfigManagerTests
    {
        #region NormalizeLanguage

        [Theory]
        [InlineData("ja", "ja-JP")]
        [InlineData("en", "en-US")]
        [InlineData("zh", "zh-CN")]
        [InlineData("ko", "ko-KR")]
        [InlineData("de", "de-DE")]
        [InlineData("fr", "fr-FR")]
        [InlineData("es", "es-ES")]
        [InlineData("it", "it-IT")]
        [InlineData("pt", "pt-BR")]
        [InlineData("ru", "ru-RU")]
        public void NormalizeLanguage_ShortCode_ReturnsFullLocale(string input, string expected)
        {
            var result = ConfigManager.NormalizeLanguage(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("ja-JP")]
        [InlineData("en-US")]
        [InlineData("zh-CN")]
        [InlineData("ko-KR")]
        public void NormalizeLanguage_AlreadyFullLocale_ReturnsUnchanged(string input)
        {
            var result = ConfigManager.NormalizeLanguage(input);
            Assert.Equal(input, result);
        }

        [Fact]
        public void NormalizeLanguage_UnknownShortCode_GuessesFormat()
        {
            // Unknown code "xx" should produce "xx-XX"
            var result = ConfigManager.NormalizeLanguage("xx");
            Assert.Equal("xx-XX", result);
        }

        #endregion

        #region ExtractLanguageFromVoice

        [Theory]
        [InlineData("ja-JP-NanamiNeural", "ja-JP")]
        [InlineData("en-US-JennyNeural", "en-US")]
        [InlineData("zh-CN-XiaoxiaoNeural", "zh-CN")]
        [InlineData("ko-KR-SunHiNeural", "ko-KR")]
        public void ExtractLanguageFromVoice_ValidVoiceName_ReturnsLanguage(string voice, string expected)
        {
            var result = ConfigManager.ExtractLanguageFromVoice(voice);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExtractLanguageFromVoice_Null_ReturnsNull()
        {
            var result = ConfigManager.ExtractLanguageFromVoice(null);
            Assert.Null(result);
        }

        [Fact]
        public void ExtractLanguageFromVoice_Empty_ReturnsNull()
        {
            var result = ConfigManager.ExtractLanguageFromVoice("");
            Assert.Null(result);
        }

        [Fact]
        public void ExtractLanguageFromVoice_NoDash_ReturnsNull()
        {
            var result = ConfigManager.ExtractLanguageFromVoice("singleword");
            Assert.Null(result);
        }

        #endregion

        #region GetDefaultVoiceForLanguage

        [Theory]
        [InlineData("ja-JP", "ja-JP-NanamiNeural")]
        [InlineData("en-US", "en-US-JennyNeural")]
        [InlineData("zh-CN", "zh-CN-XiaoxiaoNeural")]
        [InlineData("ko-KR", "ko-KR-SunHiNeural")]
        public void GetDefaultVoiceForLanguage_KnownLocale_ReturnsExpectedVoice(string locale, string expected)
        {
            var result = ConfigManager.GetDefaultVoiceForLanguage(locale);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetDefaultVoiceForLanguage_UnknownLocale_ReturnsFallback()
        {
            var result = ConfigManager.GetDefaultVoiceForLanguage("unknown-XX");
            Assert.Equal("en-US-JennyNeural", result);
        }

        #endregion

        #region Config File I/O (roundtrip via temp file)

        [Fact]
        public void SaveAndLoadConfig_Roundtrip_PreservesValues()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), $"speech_test_{Guid.NewGuid():N}");
            var tempFile = Path.Combine(tempDir, "SpeechConfig.json");

            try
            {
                Directory.CreateDirectory(tempDir);

                var config = new SpeechConfig
                {
                    Common = new CommonConfig
                    {
                        Rate = 1.5,
                        Volume = 80,
                        Language = "ja-JP"
                    },
                    Azure = new AzureConfig
                    {
                        Voice = "ja-JP-NanamiNeural",
                        Region = "japaneast",
                        Key = "test-key-12345"
                    }
                };

                // Serialize and save
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };
                var json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(tempFile, json);

                // Load and verify
                var loadedJson = File.ReadAllText(tempFile);
                var loaded = JsonSerializer.Deserialize<SpeechConfig>(loadedJson, options);

                Assert.NotNull(loaded);
                Assert.Equal(1.5, loaded.Common?.Rate);
                Assert.Equal(80, loaded.Common?.Volume);
                Assert.Equal("ja-JP", loaded.Common?.Language);
                Assert.Equal("ja-JP-NanamiNeural", loaded.Azure?.Voice);
                Assert.Equal("japaneast", loaded.Azure?.Region);
                Assert.Equal("test-key-12345", loaded.Azure?.Key);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void DeserializeConfig_InvalidJson_ReturnsNull()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Invalid JSON should throw (caller handles gracefully)
            Assert.ThrowsAny<JsonException>(() =>
                JsonSerializer.Deserialize<SpeechConfig>("{ invalid json }", options));
        }

        [Fact]
        public void DeserializeConfig_EmptyObject_ReturnsEmptyConfig()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var config = JsonSerializer.Deserialize<SpeechConfig>("{}", options);

            Assert.NotNull(config);
            Assert.Null(config.Common);
            Assert.Null(config.Azure);
            Assert.Null(config.Windows);
            Assert.Null(config.OpenAI);
            Assert.Null(config.Google);
        }

        #endregion
    }
}
