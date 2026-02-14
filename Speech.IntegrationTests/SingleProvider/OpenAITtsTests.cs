using Speech.OpenAI;

namespace Speech.IntegrationTests.SingleProvider
{
    public class OpenAITtsTests : IntegrationTestBase
    {
        [SkippableFact]
        public async Task OpenAI_TTS_ReturnsNonEmptyMp3()
        {
            Skip.IfNot(HasOpenAIKey(), "OpenAI API key not configured");

            var manager = new OpenAIAudioManager(GetOpenAIKey()!);
            var audioData = await manager.TextToSpeechAsync(
                TestConstants.HelloWorld,
                voice: TestConstants.OpenAIVoice,
                model: TestConstants.OpenAIModel);

            Assert.NotNull(audioData);
            Assert.True(audioData.Length > 0, "TTS should return non-empty audio data");
        }

        [SkippableFact]
        public async Task OpenAI_STT_RecognizesWav()
        {
            Skip.IfNot(HasOpenAIKey(), "OpenAI API key not configured");

            var manager = new OpenAIAudioManager(GetOpenAIKey()!);

            // TTS → MP3 → WAV → STT
            var mp3 = await manager.TextToSpeechAsync(TestConstants.HelloWorld);
            var wav = Speech.Core.AudioConverter.Mp3ToWav(mp3);
            var result = await manager.SpeechToTextAsync(wav, fileName: "audio.wav", language: "en");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
