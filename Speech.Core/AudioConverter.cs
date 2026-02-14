using NAudio.Wave;

namespace Speech.Core
{
    /// <summary>
    /// Converts audio between formats for TTS→STT roundtrip integration.
    /// </summary>
    public static class AudioConverter
    {
        /// <summary>
        /// Cloud TTS (MP3) → OpenAI Whisper STT (WAV 16kHz/16bit/mono)
        /// </summary>
        public static byte[] Mp3ToWav(byte[] mp3Data, int sampleRate = 16000, int bitsPerSample = 16, int channels = 1)
        {
            if (mp3Data == null || mp3Data.Length == 0)
                throw new ArgumentException("MP3 data is empty.", nameof(mp3Data));

            using var mp3Stream = new MemoryStream(mp3Data);
            using var mp3Reader = new Mp3FileReader(mp3Stream);

            var targetFormat = new WaveFormat(sampleRate, bitsPerSample, channels);
            using var resampler = new MediaFoundationResampler(mp3Reader, targetFormat);
            resampler.ResamplerQuality = 60;

            using var outputStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(outputStream, resampler);
            return outputStream.ToArray();
        }

        /// <summary>
        /// Cloud TTS (MP3) → Google/Azure STT (raw PCM 16kHz/16bit/mono)
        /// </summary>
        public static byte[] Mp3ToPcm(byte[] mp3Data, int sampleRate = 16000, int bitsPerSample = 16, int channels = 1)
        {
            if (mp3Data == null || mp3Data.Length == 0)
                throw new ArgumentException("MP3 data is empty.", nameof(mp3Data));

            using var mp3Stream = new MemoryStream(mp3Data);
            using var mp3Reader = new Mp3FileReader(mp3Stream);

            var targetFormat = new WaveFormat(sampleRate, bitsPerSample, channels);
            using var resampler = new MediaFoundationResampler(mp3Reader, targetFormat);
            resampler.ResamplerQuality = 60;

            using var outputStream = new MemoryStream();
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }
            return outputStream.ToArray();
        }

        /// <summary>
        /// Windows TTS (WAV) → Google/Azure STT (raw PCM)
        /// </summary>
        public static byte[] WavToPcm(byte[] wavData, int targetSampleRate = 16000, int targetBitsPerSample = 16, int targetChannels = 1)
        {
            if (wavData == null || wavData.Length == 0)
                throw new ArgumentException("WAV data is empty.", nameof(wavData));

            using var wavStream = new MemoryStream(wavData);
            using var wavReader = new WaveFileReader(wavStream);

            var targetFormat = new WaveFormat(targetSampleRate, targetBitsPerSample, targetChannels);

            // If already in the target format, just read the raw data
            if (wavReader.WaveFormat.SampleRate == targetSampleRate &&
                wavReader.WaveFormat.BitsPerSample == targetBitsPerSample &&
                wavReader.WaveFormat.Channels == targetChannels)
            {
                using var outputStream = new MemoryStream();
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = wavReader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                }
                return outputStream.ToArray();
            }

            using var resampler = new MediaFoundationResampler(wavReader, targetFormat);
            resampler.ResamplerQuality = 60;

            using var resampledStream = new MemoryStream();
            var resampleBuffer = new byte[4096];
            int resampledBytesRead;
            while ((resampledBytesRead = resampler.Read(resampleBuffer, 0, resampleBuffer.Length)) > 0)
            {
                resampledStream.Write(resampleBuffer, 0, resampledBytesRead);
            }
            return resampledStream.ToArray();
        }

        /// <summary>
        /// Raw PCM → WAV with header (Google recording → OpenAI/Windows STT)
        /// </summary>
        public static byte[] PcmToWav(byte[] pcmData, int sampleRate = 16000, int bitsPerSample = 16, int channels = 1)
        {
            if (pcmData == null || pcmData.Length == 0)
                throw new ArgumentException("PCM data is empty.", nameof(pcmData));

            var format = new WaveFormat(sampleRate, bitsPerSample, channels);

            using var outputStream = new MemoryStream();
            using (var writer = new WaveFileWriter(outputStream, format))
            {
                writer.Write(pcmData, 0, pcmData.Length);
            }
            return outputStream.ToArray();
        }
    }
}
