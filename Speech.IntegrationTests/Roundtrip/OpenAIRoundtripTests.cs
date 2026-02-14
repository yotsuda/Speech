using Speech.Core;
using Speech.OpenAI;

namespace Speech.IntegrationTests.Roundtrip
{
    public class OpenAIRoundtripTests : IntegrationTestBase
    {
        [SkippableFact]
        public async Task OpenAI_TTS_STT_Roundtrip()
        {
            Skip.IfNot(HasOpenAIKey(), "OpenAI API key not configured");

            var manager = new OpenAIAudioManager(GetOpenAIKey()!);

            // TTS: "Hello world" → MP3
            var mp3 = await manager.TextToSpeechAsync(
                TestConstants.HelloWorld,
                voice: TestConstants.OpenAIVoice,
                model: TestConstants.OpenAIModel);
            Assert.True(mp3.Length > 0);

            // Convert: MP3 → WAV (OpenAI Whisper expects WAV)
            var wav = AudioConverter.Mp3ToWav(mp3);

            // STT: WAV → text
            var result = await manager.SpeechToTextAsync(wav, fileName: "audio.wav", language: "en");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }
    }
}
