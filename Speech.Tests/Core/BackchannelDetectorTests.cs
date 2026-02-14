using Speech.Core;
using Xunit;

namespace Speech.Tests.Core
{
    public class BackchannelDetectorTests
    {
        #region Japanese backchannels

        [Theory]
        [InlineData("うん")]
        [InlineData("ええ")]
        [InlineData("はい")]
        [InlineData("へー")]
        [InlineData("ほー")]
        [InlineData("なるほど")]
        [InlineData("そうですか")]
        public void IsBackchannel_JapaneseBackchannel_ShortDuration_ReturnsTrue(string text)
        {
            Assert.True(BackchannelDetector.IsBackchannel(text, 0.5));
        }

        #endregion

        #region English backchannels

        [Theory]
        [InlineData("uh-huh")]
        [InlineData("yeah")]
        [InlineData("okay")]
        [InlineData("right")]
        [InlineData("i see")]
        [InlineData("mm-hmm")]
        [InlineData("yes")]
        [InlineData("yep")]
        [InlineData("yup")]
        public void IsBackchannel_EnglishBackchannel_ShortDuration_ReturnsTrue(string text)
        {
            Assert.True(BackchannelDetector.IsBackchannel(text, 0.5));
        }

        #endregion

        #region Normal speech (not backchannel)

        [Theory]
        [InlineData("今日の天気は")]
        [InlineData("Hello, how are you?")]
        [InlineData("Let me think about that")]
        public void IsBackchannel_NormalSpeech_ReturnsFalse(string text)
        {
            Assert.False(BackchannelDetector.IsBackchannel(text, 0.5));
        }

        #endregion

        #region Duration threshold

        [Theory]
        [InlineData("うん")]
        [InlineData("yeah")]
        [InlineData("okay")]
        public void IsBackchannel_BackchannelWord_LongDuration_ReturnsFalse(string text)
        {
            // Duration > 0.8 seconds should not be considered backchannel
            Assert.False(BackchannelDetector.IsBackchannel(text, 1.0));
        }

        [Fact]
        public void IsBackchannel_AtExactThreshold_ReturnsFalse()
        {
            // Duration > MaxBackchannelDuration (0.8), so 0.81 should be false
            Assert.False(BackchannelDetector.IsBackchannel("yeah", 0.81));
        }

        [Fact]
        public void IsBackchannel_JustUnderThreshold_ReturnsTrue()
        {
            Assert.True(BackchannelDetector.IsBackchannel("yeah", 0.8));
        }

        #endregion

        #region Case insensitivity

        [Theory]
        [InlineData("Yeah")]
        [InlineData("YEAH")]
        [InlineData("Okay")]
        [InlineData("OKAY")]
        [InlineData("Yes")]
        [InlineData("YES")]
        public void IsBackchannel_MixedCase_ReturnsTrue(string text)
        {
            Assert.True(BackchannelDetector.IsBackchannel(text, 0.5));
        }

        #endregion

        #region Edge cases

        [Fact]
        public void IsBackchannel_NullText_ReturnsFalse()
        {
            Assert.False(BackchannelDetector.IsBackchannel(null!, 0.5));
        }

        [Fact]
        public void IsBackchannel_EmptyText_ReturnsFalse()
        {
            Assert.False(BackchannelDetector.IsBackchannel("", 0.5));
        }

        [Fact]
        public void IsBackchannel_WhitespaceOnly_ReturnsFalse()
        {
            Assert.False(BackchannelDetector.IsBackchannel("   ", 0.5));
        }

        [Fact]
        public void MaxBackchannelDuration_Is08()
        {
            Assert.Equal(0.8, BackchannelDetector.MaxBackchannelDuration);
        }

        #endregion

        #region MatchesBackchannelPattern (duration-independent)

        [Theory]
        [InlineData("うん", true)]
        [InlineData("yeah", true)]
        [InlineData("Hello world", false)]
        [InlineData("", false)]
        public void MatchesBackchannelPattern_VariousInputs(string text, bool expected)
        {
            Assert.Equal(expected, BackchannelDetector.MatchesBackchannelPattern(text));
        }

        #endregion
    }
}
