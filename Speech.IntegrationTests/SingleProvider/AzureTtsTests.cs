using Speech.Azure;

namespace Speech.IntegrationTests.SingleProvider
{
    public class AzureTtsTests : IntegrationTestBase
    {
        [SkippableFact]
        public async Task Azure_TTS_ReturnsNonEmptyMp3()
        {
            Skip.IfNot(HasAzureKey(), "Azure API key not configured");

            var manager = AzureAudioManager.GetInstance(GetAzureKey()!, GetAzureRegion());
            var audioData = await manager.SynthesizeToByteArrayAsync(TestConstants.HelloWorldSsml);

            Assert.NotNull(audioData);
            Assert.True(audioData.Length > 0, "TTS should return non-empty audio data");
        }
    }
}
