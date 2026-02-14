using Speech.Core;
using Speech.Google;

namespace Speech.IntegrationTests.Roundtrip
{
    public class GoogleRoundtripTests : IntegrationTestBase
    {
        [SkippableFact]
        public async Task Google_TTS_STT_Roundtrip()
        {
            Skip.IfNot(HasGoogleCredential(), "Google credential not configured");

            var manager = new GoogleAudioManager(GetGoogleCredential()!);

            // TTS: "Hello world" → MP3
            var mp3 = await manager.TextToSpeechAsync(
                TestConstants.HelloWorld,
                voiceName: TestConstants.GoogleVoice,
                languageCode: TestConstants.GoogleLanguage);
            Assert.True(mp3.Length > 0);

            // Convert: MP3 → PCM (Google STT expects LINEAR16)
            var pcm = AudioConverter.Mp3ToPcm(mp3);

            // STT: PCM → text
            var result = await manager.SpeechToTextAsync(pcm, languageCode: TestConstants.GoogleLanguage);

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }
    }
}
