using Speech.Core;
using Xunit;

namespace Speech.Tests.Core
{
    public class MaskKeyTests
    {
        [Fact]
        public void MaskKey_NormalKey_MasksAllButLastFour()
        {
            var result = GetSpeechConfigCmdlet.MaskKey("sk-1234567890");
            Assert.Equal("*********7890", result);
        }

        [Fact]
        public void MaskKey_LongKey_MasksCorrectly()
        {
            var result = GetSpeechConfigCmdlet.MaskKey("abcdefghijklmnop");
            Assert.Equal("************mnop", result);
        }

        [Fact]
        public void MaskKey_ExactlyNineChars_MasksAllButLastFour()
        {
            var result = GetSpeechConfigCmdlet.MaskKey("123456789");
            Assert.Equal("*****6789", result);
        }

        [Fact]
        public void MaskKey_EightChars_FullyMasked()
        {
            // ≤ 8 chars → fully masked
            var result = GetSpeechConfigCmdlet.MaskKey("12345678");
            Assert.Equal("********", result);
        }

        [Fact]
        public void MaskKey_ShortKey_FullyMasked()
        {
            var result = GetSpeechConfigCmdlet.MaskKey("abc");
            Assert.Equal("***", result);
        }

        [Fact]
        public void MaskKey_SingleChar_FullyMasked()
        {
            var result = GetSpeechConfigCmdlet.MaskKey("x");
            Assert.Equal("*", result);
        }

        [Fact]
        public void MaskKey_Null_ReturnsNotSet()
        {
            var result = GetSpeechConfigCmdlet.MaskKey(null);
            Assert.Equal("(not set)", result);
        }

        [Fact]
        public void MaskKey_Empty_ReturnsNotSet()
        {
            var result = GetSpeechConfigCmdlet.MaskKey("");
            Assert.Equal("(not set)", result);
        }
    }
}
