using Speech.Amazon;
using Xunit;

namespace Speech.Tests.Amazon
{
    public class AmazonTests
    {
        #region EscapeXml

        [Fact]
        public void EscapeXml_Ampersand_IsEscaped()
        {
            var result = AmazonAudioManager.EscapeXml("A & B");
            Assert.Equal("A &amp; B", result);
        }

        [Fact]
        public void EscapeXml_LessThan_IsEscaped()
        {
            var result = AmazonAudioManager.EscapeXml("1 < 2");
            Assert.Equal("1 &lt; 2", result);
        }

        [Fact]
        public void EscapeXml_GreaterThan_IsEscaped()
        {
            var result = AmazonAudioManager.EscapeXml("3 > 0");
            Assert.Equal("3 &gt; 0", result);
        }

        [Fact]
        public void EscapeXml_DoubleQuote_IsEscaped()
        {
            var result = AmazonAudioManager.EscapeXml("say \"hello\"");
            Assert.Equal("say &quot;hello&quot;", result);
        }

        [Fact]
        public void EscapeXml_SingleQuote_IsEscaped()
        {
            var result = AmazonAudioManager.EscapeXml("it's");
            Assert.Equal("it&apos;s", result);
        }

        [Fact]
        public void EscapeXml_AllSpecialChars_AreEscaped()
        {
            var result = AmazonAudioManager.EscapeXml("A & B < C > D \"E\" 'F'");
            Assert.DoesNotContain("&", result.Replace("&amp;", "").Replace("&lt;", "").Replace("&gt;", "").Replace("&quot;", "").Replace("&apos;", ""));
        }

        [Fact]
        public void EscapeXml_NoSpecialChars_Unchanged()
        {
            var result = AmazonAudioManager.EscapeXml("Hello world");
            Assert.Equal("Hello world", result);
        }

        [Fact]
        public void EscapeXml_Empty_ReturnsEmpty()
        {
            var result = AmazonAudioManager.EscapeXml("");
            Assert.Equal("", result);
        }

        #endregion

        #region GetDefaultVoice

        [Fact]
        public void GetDefaultVoice_ReturnsNonNullString()
        {
            var result = OutAmazonSpeechCmdlet.GetDefaultVoice();
            Assert.False(string.IsNullOrEmpty(result));
        }

        #endregion

        #region AmazonVoiceInfo

        [Fact]
        public void AmazonVoiceInfo_DefaultValues_AreEmpty()
        {
            var info = new AmazonVoiceInfo();
            Assert.Equal("", info.Id);
            Assert.Equal("", info.Name);
            Assert.Equal("", info.Gender);
            Assert.Equal("", info.LanguageCode);
            Assert.Equal("", info.LanguageName);
            Assert.Empty(info.SupportedEngines);
        }

        [Fact]
        public void AmazonVoiceInfo_Properties_CanBeSet()
        {
            var info = new AmazonVoiceInfo
            {
                Id = "Mizuki",
                Name = "Mizuki",
                Gender = "Female",
                LanguageCode = "ja-JP",
                LanguageName = "Japanese",
                SupportedEngines = new List<string> { "neural", "standard" }
            };

            Assert.Equal("Mizuki", info.Id);
            Assert.Equal("ja-JP", info.LanguageCode);
            Assert.Equal(2, info.SupportedEngines.Count);
        }

        #endregion
    }
}
