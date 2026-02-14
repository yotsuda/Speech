using Speech.Windows;

namespace Speech.IntegrationTests.SingleProvider
{
    public class WindowsTtsTests : IntegrationTestBase
    {
        [SkippableFact]
        public void Windows_TTS_ReturnsNonEmptyWav()
        {
            Skip.IfNot(WindowsAudioManager.IsWindowsPlatform(), "Windows platform required");

            var audioData = WindowsAudioManager.SynthesizeToByteArray(TestConstants.HelloWorld);

            Assert.NotNull(audioData);
            Assert.True(audioData.Length > 0, "TTS should return non-empty audio data");
            // Verify WAV header
            Assert.Equal((byte)'R', audioData[0]);
            Assert.Equal((byte)'I', audioData[1]);
            Assert.Equal((byte)'F', audioData[2]);
            Assert.Equal((byte)'F', audioData[3]);
        }

        [SkippableFact]
        public void Windows_STT_RecognizesWav()
        {
            Skip.IfNot(WindowsAudioManager.IsWindowsPlatform(), "Windows platform required");

            var wav = WindowsAudioManager.SynthesizeToByteArray(TestConstants.HelloWorld);
            var result = WindowsAudioManager.RecognizeFromBytes(wav, culture: "en-US");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
