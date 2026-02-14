using Speech.Google;

namespace Speech.IntegrationTests.SingleProvider
{
    public class GoogleTtsTests : IntegrationTestBase
    {
        [SkippableFact]
        public async Task Google_TTS_ReturnsNonEmptyMp3()
        {
            Skip.IfNot(HasGoogleCredential(), "Google credential not configured");

            var manager = new GoogleAudioManager(GetGoogleCredential()!);
            var audioData = await manager.TextToSpeechAsync(
                TestConstants.HelloWorld,
                voiceName: TestConstants.GoogleVoice,
                languageCode: TestConstants.GoogleLanguage);

            Assert.NotNull(audioData);
            Assert.True(audioData.Length > 0, "TTS should return non-empty audio data");
        }
    }
}
