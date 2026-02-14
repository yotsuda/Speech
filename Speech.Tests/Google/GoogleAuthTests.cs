using System.Text;
using Speech.Google;
using Xunit;

namespace Speech.Tests.Google
{
    public class GoogleAuthTests
    {
        #region Base64UrlEncode

        [Fact]
        public void Base64UrlEncode_ReplacesPlus_WithDash()
        {
            // Find bytes that produce '+' in standard Base64
            // 0xFB -> standard Base64 contains '+'
            var data = new byte[] { 0xFB, 0xEF, 0xBE };
            var result = GoogleAuthManager.Base64UrlEncode(data);

            Assert.DoesNotContain("+", result);
        }

        [Fact]
        public void Base64UrlEncode_ReplacesSlash_WithUnderscore()
        {
            // Find bytes that produce '/' in standard Base64
            // 0xFF -> standard Base64 contains '/'
            var data = new byte[] { 0xFF, 0xFF, 0xFE };
            var result = GoogleAuthManager.Base64UrlEncode(data);

            Assert.DoesNotContain("/", result);
        }

        [Fact]
        public void Base64UrlEncode_RemovesPadding()
        {
            // "a" in Base64 is "YQ==" - has padding
            var data = Encoding.UTF8.GetBytes("a");
            var result = GoogleAuthManager.Base64UrlEncode(data);

            Assert.DoesNotContain("=", result);
            Assert.Equal("YQ", result);
        }

        [Fact]
        public void Base64UrlEncode_EmptyArray_ReturnsEmpty()
        {
            var result = GoogleAuthManager.Base64UrlEncode(Array.Empty<byte>());
            Assert.Equal("", result);
        }

        [Fact]
        public void Base64UrlEncode_KnownInput_ReturnsExpected()
        {
            // "Hello" → Base64 "SGVsbG8=" → Base64URL "SGVsbG8"
            var data = Encoding.UTF8.GetBytes("Hello");
            var result = GoogleAuthManager.Base64UrlEncode(data);
            Assert.Equal("SGVsbG8", result);
        }

        [Fact]
        public void Base64UrlEncode_JsonPayload_ProducesUrlSafeOutput()
        {
            // Simulate a JWT-like payload
            var json = "{\"iss\":\"test@test.iam.gserviceaccount.com\",\"aud\":\"https://oauth2.googleapis.com/token\"}";
            var data = Encoding.UTF8.GetBytes(json);
            var result = GoogleAuthManager.Base64UrlEncode(data);

            Assert.DoesNotContain("+", result);
            Assert.DoesNotContain("/", result);
            Assert.DoesNotContain("=", result);
        }

        #endregion

        #region Token cache behavior (structural test)

        [Fact]
        public void GoogleAuthManager_RequiresCredentialFile()
        {
            // Attempting to create with a non-existent file should throw
            Assert.ThrowsAny<Exception>(() =>
                new GoogleAuthManager("/nonexistent/path/credentials.json"));
        }

        #endregion
    }
}
