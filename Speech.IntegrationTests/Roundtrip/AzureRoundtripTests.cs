using Speech.Core;
using Speech.Azure;

namespace Speech.IntegrationTests.Roundtrip
{
    public class AzureRoundtripTests : IntegrationTestBase
    {
        [SkippableFact]
        public async Task Azure_TTS_STT_Roundtrip()
        {
            Skip.IfNot(HasAzureKey(), "Azure API key not configured");

            var manager = AzureAudioManager.GetInstance(GetAzureKey()!, GetAzureRegion());

            // TTS: SSML → MP3
            var mp3 = await manager.SynthesizeToByteArrayAsync(TestConstants.HelloWorldSsml);
            Assert.True(mp3.Length > 0);

            // Convert: MP3 → PCM (Azure SDK expects raw PCM 16kHz/16bit/mono)
            var pcm = AudioConverter.Mp3ToPcm(mp3);

            // STT: PCM → text
            var result = await manager.RecognizeFromBytesAsync(pcm, language: "en-US");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }
    }
}
