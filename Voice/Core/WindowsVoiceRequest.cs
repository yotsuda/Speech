using System;
using System.IO;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Voice.Core
{
    /// <summary>
    /// Windows音声合成リクエスト
    /// </summary>
    public class WindowsVoiceRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? Voice { get; set; }
        public int Rate { get; set; } = 0;
        public int Volume { get; set; } = 100;

        public async Task PlayAsync(CancellationToken cancellationToken)
        {
            using var synthesizer = new SpeechSynthesizer();

            if (!string.IsNullOrEmpty(Voice))
            {
                try
                {
                    synthesizer.SelectVoice(Voice);
                }
                catch (ArgumentException)
                {
                    // 音声が見つからない場合はデフォルト使用
                }
            }

            synthesizer.Rate = Rate;
            synthesizer.Volume = Volume;

            using var memoryStream = new MemoryStream();
            synthesizer.SetOutputToWaveStream(memoryStream);
            synthesizer.Speak(Text);

            memoryStream.Position = 0;
            await PlayAudioAsync(memoryStream, cancellationToken);
        }

        private async Task PlayAudioAsync(Stream audioStream, CancellationToken cancellationToken)
        {
            using var reader = new WaveFileReader(audioStream);
            using var waveOut = new WaveOutEvent();

            waveOut.Init(reader);
            waveOut.Play();

            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(50, cancellationToken);
            }
        }
    }
}
