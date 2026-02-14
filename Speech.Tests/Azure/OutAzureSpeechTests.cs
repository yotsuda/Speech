using System.Xml.Linq;
using Speech.Azure;
using Speech.Core;
using Xunit;

namespace Speech.Tests.Azure
{
    public class OutAzureSpeechTests
    {
        private OutAzureVoiceCmdlet CreateCmdlet()
        {
            return new OutAzureVoiceCmdlet();
        }

        #region BuildSsml

        [Fact]
        public void BuildSsml_BasicInput_GeneratesValidSsml()
        {
            var cmdlet = CreateCmdlet();
            var ssml = cmdlet.BuildSsml("Hello world", "en-US-JennyNeural", 1.0, 100, 0);

            // Should be valid XML
            var doc = XDocument.Parse(ssml);
            Assert.NotNull(doc.Root);

            // Check root element
            Assert.Equal("speak", doc.Root.Name.LocalName);
            Assert.Contains("en-US", ssml);
            Assert.Contains("en-US-JennyNeural", ssml);
            Assert.Contains("Hello world", ssml);
        }

        [Fact]
        public void BuildSsml_JapaneseVoice_SetsCorrectLanguage()
        {
            var cmdlet = CreateCmdlet();
            var ssml = cmdlet.BuildSsml("こんにちは", "ja-JP-NanamiNeural", 1.0, 100, 0);

            Assert.Contains("xml:lang=\"ja-JP\"", ssml);
            Assert.Contains("ja-JP-NanamiNeural", ssml);
        }

        [Fact]
        public void BuildSsml_CustomRate_SetsRateAttribute()
        {
            var cmdlet = CreateCmdlet();

            // Rate 1.5 → +50%
            var ssml = cmdlet.BuildSsml("test", "en-US-JennyNeural", 1.5, 100, 0);
            Assert.Contains("rate=\"+50%\"", ssml);
        }

        [Fact]
        public void BuildSsml_SlowRate_SetsNegativeRate()
        {
            var cmdlet = CreateCmdlet();

            // Rate 0.5 → -50%
            var ssml = cmdlet.BuildSsml("test", "en-US-JennyNeural", 0.5, 100, 0);
            Assert.Contains("rate=\"-50%\"", ssml);
        }

        [Fact]
        public void BuildSsml_DefaultRate_SetsZeroPercent()
        {
            var cmdlet = CreateCmdlet();

            // Rate 1.0 → +0%
            var ssml = cmdlet.BuildSsml("test", "en-US-JennyNeural", 1.0, 100, 0);
            Assert.Contains("rate=\"+0%\"", ssml);
        }

        [Fact]
        public void BuildSsml_Volume_SetsVolumeAttribute()
        {
            var cmdlet = CreateCmdlet();
            var ssml = cmdlet.BuildSsml("test", "en-US-JennyNeural", 1.0, 80, 0);
            Assert.Contains("volume=\"80%\"", ssml);
        }

        [Fact]
        public void BuildSsml_WithPitch_IncludesPitchAttribute()
        {
            var cmdlet = CreateCmdlet();
            var ssml = cmdlet.BuildSsml("test", "en-US-JennyNeural", 1.0, 100, 10);
            Assert.Contains("pitch=\"+10Hz\"", ssml);
        }

        [Fact]
        public void BuildSsml_NegativePitch_IncludesNegativePitch()
        {
            var cmdlet = CreateCmdlet();
            var ssml = cmdlet.BuildSsml("test", "en-US-JennyNeural", 1.0, 100, -5);
            Assert.Contains("pitch=\"-5Hz\"", ssml);
        }

        [Fact]
        public void BuildSsml_ZeroPitch_OmitsPitchAttribute()
        {
            var cmdlet = CreateCmdlet();
            var ssml = cmdlet.BuildSsml("test", "en-US-JennyNeural", 1.0, 100, 0);
            Assert.DoesNotContain("pitch=", ssml);
        }

        [Fact]
        public void BuildSsml_XmlSpecialChars_AreEscaped()
        {
            var cmdlet = CreateCmdlet();
            var ssml = cmdlet.BuildSsml("1 < 2 & 3 > 0", "en-US-JennyNeural", 1.0, 100, 0);

            // The text should be XML-escaped
            Assert.DoesNotContain("< 2", ssml);
            Assert.DoesNotContain("& 3", ssml);
            Assert.DoesNotContain("> 0", ssml);
            Assert.Contains("&lt;", ssml);
            Assert.Contains("&amp;", ssml);
            Assert.Contains("&gt;", ssml);

            // Should still be valid XML
            var doc = XDocument.Parse(ssml);
            Assert.NotNull(doc);
        }

        #endregion

        #region DetectTextLanguage

        [Fact]
        public void DetectTextLanguage_JapaneseText_ReturnsJaJP()
        {
            var cmdlet = CreateCmdlet();
            var result = cmdlet.DetectTextLanguage("こんにちは世界");
            Assert.Equal("ja-JP", result);
        }

        [Fact]
        public void DetectTextLanguage_KoreanText_ReturnsKoKR()
        {
            var cmdlet = CreateCmdlet();
            var result = cmdlet.DetectTextLanguage("안녕하세요");
            Assert.Equal("ko-KR", result);
        }

        [Fact]
        public void DetectTextLanguage_ChineseText_ReturnsZhCN()
        {
            var cmdlet = CreateCmdlet();
            // Pure CJK without kana → Chinese
            var result = cmdlet.DetectTextLanguage("你好世界");
            Assert.Equal("zh-CN", result);
        }

        [Fact]
        public void DetectTextLanguage_EnglishText_ReturnsEnUS()
        {
            var cmdlet = CreateCmdlet();
            var result = cmdlet.DetectTextLanguage("Hello world");
            Assert.Equal("en-US", result);
        }

        [Fact]
        public void DetectTextLanguage_Null_ReturnsNull()
        {
            var cmdlet = CreateCmdlet();
            var result = cmdlet.DetectTextLanguage(null);
            Assert.Null(result);
        }

        [Fact]
        public void DetectTextLanguage_Empty_ReturnsNull()
        {
            var cmdlet = CreateCmdlet();
            var result = cmdlet.DetectTextLanguage("");
            Assert.Null(result);
        }

        [Fact]
        public void DetectTextLanguage_KanaOverCjk_ReturnsJapanese()
        {
            var cmdlet = CreateCmdlet();
            // Text with both kana and CJK → Japanese (kana takes priority)
            var result = cmdlet.DetectTextLanguage("漢字とひらがな");
            Assert.Equal("ja-JP", result);
        }

        #endregion

        #region ResolveVoice

        [Fact]
        public void ResolveVoice_VoiceParameter_TakesPriority()
        {
            var cmdlet = CreateCmdlet();
            cmdlet.Voice = "custom-voice";

            var config = new SpeechConfig
            {
                Azure = new AzureConfig { Voice = "config-voice" },
                Common = new CommonConfig { Language = "ja" }
            };

            var result = cmdlet.ResolveVoice(config);
            Assert.Equal("custom-voice", result);
        }

        [Fact]
        public void ResolveVoice_NoParam_UsesConfigAzureVoice()
        {
            var cmdlet = CreateCmdlet();

            var config = new SpeechConfig
            {
                Azure = new AzureConfig { Voice = "ja-JP-NanamiNeural" }
            };

            var result = cmdlet.ResolveVoice(config);
            Assert.Equal("ja-JP-NanamiNeural", result);
        }

        [Fact]
        public void ResolveVoice_LanguageParameter_UsesDefaultVoiceForLanguage()
        {
            var cmdlet = CreateCmdlet();
            cmdlet.Language = "ko";

            var config = new SpeechConfig();

            var result = cmdlet.ResolveVoice(config);
            Assert.Equal("ko-KR-SunHiNeural", result);
        }

        [Fact]
        public void ResolveVoice_ConfigLanguage_UsesDefaultVoice()
        {
            var cmdlet = CreateCmdlet();

            var config = new SpeechConfig
            {
                Common = new CommonConfig { Language = "ja" }
            };

            var result = cmdlet.ResolveVoice(config);
            Assert.Equal("ja-JP-NanamiNeural", result);
        }

        [Fact]
        public void ResolveVoice_NothingSet_ReturnsFallback()
        {
            var cmdlet = CreateCmdlet();
            var config = new SpeechConfig();

            var result = cmdlet.ResolveVoice(config);
            Assert.Equal("en-US-JennyNeural", result);
        }

        #endregion
    }
}
