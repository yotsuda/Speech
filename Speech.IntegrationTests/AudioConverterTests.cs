using Speech.Core;

namespace Speech.IntegrationTests
{
    public class AudioConverterTests
    {
        /// <summary>
        /// Create a minimal valid MP3 is hard without encoding, so we test PcmToWav/WavToPcm
        /// which don't need external audio. MP3 conversion is tested in provider integration tests.
        /// </summary>

        [Fact]
        public void PcmToWav_ProducesValidWavHeader()
        {
            // Arrange: 1 second of silence at 16kHz/16bit/mono = 32000 bytes
            var pcm = new byte[32000];

            // Act
            var wav = AudioConverter.PcmToWav(pcm, sampleRate: 16000, bitsPerSample: 16, channels: 1);

            // Assert: WAV starts with "RIFF"
            Assert.True(wav.Length > 44, "WAV should be longer than 44-byte header");
            Assert.Equal((byte)'R', wav[0]);
            Assert.Equal((byte)'I', wav[1]);
            Assert.Equal((byte)'F', wav[2]);
            Assert.Equal((byte)'F', wav[3]);
        }

        [Fact]
        public void PcmToWav_SampleRateIsCorrect()
        {
            var pcm = new byte[32000];
            var wav = AudioConverter.PcmToWav(pcm, sampleRate: 16000, bitsPerSample: 16, channels: 1);

            // Sample rate is at bytes 24-27 in WAV header (little-endian)
            var sampleRate = BitConverter.ToInt32(wav, 24);
            Assert.Equal(16000, sampleRate);
        }

        [Fact]
        public void PcmToWav_ThenWavToPcm_Roundtrip()
        {
            // Arrange: known PCM data
            var originalPcm = new byte[32000];
            new Random(42).NextBytes(originalPcm);

            // Act: PCM → WAV → PCM
            var wav = AudioConverter.PcmToWav(originalPcm, sampleRate: 16000, bitsPerSample: 16, channels: 1);
            var roundtrippedPcm = AudioConverter.WavToPcm(wav, targetSampleRate: 16000, targetBitsPerSample: 16, targetChannels: 1);

            // Assert: data should be identical
            Assert.Equal(originalPcm.Length, roundtrippedPcm.Length);
            Assert.Equal(originalPcm, roundtrippedPcm);
        }

        [Fact]
        public void WavToPcm_RemovesWavHeader()
        {
            // Arrange: create WAV from PCM
            var pcm = new byte[32000];
            var wav = AudioConverter.PcmToWav(pcm, sampleRate: 16000, bitsPerSample: 16, channels: 1);

            // Act
            var result = AudioConverter.WavToPcm(wav);

            // Assert: result should not start with "RIFF"
            Assert.True(result.Length > 0);
            // The result should be raw PCM (same size as original)
            Assert.Equal(pcm.Length, result.Length);
        }

        [Fact]
        public void Mp3ToWav_ThrowsOnEmptyData()
        {
            Assert.Throws<ArgumentException>(() => AudioConverter.Mp3ToWav(Array.Empty<byte>()));
        }

        [Fact]
        public void Mp3ToPcm_ThrowsOnEmptyData()
        {
            Assert.Throws<ArgumentException>(() => AudioConverter.Mp3ToPcm(Array.Empty<byte>()));
        }

        [Fact]
        public void WavToPcm_ThrowsOnEmptyData()
        {
            Assert.Throws<ArgumentException>(() => AudioConverter.WavToPcm(Array.Empty<byte>()));
        }

        [Fact]
        public void PcmToWav_ThrowsOnEmptyData()
        {
            Assert.Throws<ArgumentException>(() => AudioConverter.PcmToWav(Array.Empty<byte>()));
        }

        [Fact]
        public void PcmToWav_ThrowsOnNull()
        {
            Assert.Throws<ArgumentException>(() => AudioConverter.PcmToWav(null!));
        }
    }
}
