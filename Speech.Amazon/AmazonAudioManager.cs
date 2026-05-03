using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Amazon.TranscribeStreaming;
using Amazon.TranscribeStreaming.Model;

namespace Speech.Amazon
{
    public class AmazonAudioManager : IDisposable
    {
        private readonly AmazonPollyClient _pollyClient;
        private readonly string _region;
        private readonly BasicAWSCredentials _credentials;

        public AmazonAudioManager(string accessKey, string secretKey, string region)
        {
            _credentials = new BasicAWSCredentials(accessKey, secretKey);
            _region = region;
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            _pollyClient = new AmazonPollyClient(_credentials, regionEndpoint);
        }

        public async Task<byte[]> TextToSpeechAsync(string text, string voiceId, string? languageCode = null, double? rate = null)
        {
            var request = new SynthesizeSpeechRequest
            {
                Text = text,
                VoiceId = voiceId,
                OutputFormat = OutputFormat.Mp3,
                Engine = Engine.Neural
            };

            if (!string.IsNullOrEmpty(languageCode))
            {
                request.LanguageCode = languageCode;
            }

            // Apply rate via SSML if specified
            if (rate.HasValue && Math.Abs(rate.Value - 1.0) > 0.01)
            {
                var ratePercent = (int)(rate.Value * 100);
                request.Text = $"<speak><prosody rate=\"{ratePercent}%\">{EscapeXml(text)}</prosody></speak>";
                request.TextType = TextType.Ssml;
            }

            try
            {
                var response = await _pollyClient.SynthesizeSpeechAsync(request);

                using var ms = new MemoryStream();
                await response.AudioStream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch (AmazonPollyException ex) when (ex.ErrorCode == "InvalidParameterValue" && ex.Message.Contains("engine"))
            {
                // Fallback to standard engine if neural is not available for this voice
                request.Engine = Engine.Standard;
                var response = await _pollyClient.SynthesizeSpeechAsync(request);
                using var ms = new MemoryStream();
                await response.AudioStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task<List<AmazonVoiceInfo>> GetVoicesAsync(string? languageCode = null)
        {
            var request = new DescribeVoicesRequest();
            if (!string.IsNullOrEmpty(languageCode))
            {
                request.LanguageCode = languageCode;
            }

            var voices = new List<AmazonVoiceInfo>();
            string? nextToken = null;

            do
            {
                request.NextToken = nextToken;
                var response = await _pollyClient.DescribeVoicesAsync(request);

                foreach (var voice in response.Voices)
                {
                    voices.Add(new AmazonVoiceInfo
                    {
                        Id = voice.Id?.Value ?? "",
                        Name = voice.Name ?? "",
                        Gender = voice.Gender?.Value ?? "",
                        LanguageCode = voice.LanguageCode?.Value ?? "",
                        LanguageName = voice.LanguageName ?? "",
                        SupportedEngines = voice.SupportedEngines ?? new List<string>()
                    });
                }

                nextToken = response.NextToken;
            } while (!string.IsNullOrEmpty(nextToken));

            return voices.OrderBy(v => v.LanguageCode).ThenBy(v => v.Name).ToList();
        }

        public async Task<string> TranscribeStreamAsync(
            MemoryStream audioStream,
            string languageCode,
            CancellationToken cancellationToken = default)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(_region);
            using var transcribeClient = new AmazonTranscribeStreamingClient(_credentials, regionEndpoint);

            var audioData = audioStream.ToArray();
            int sendOffset = 0;
            const int chunkSize = 8000; // ~250ms of 16kHz 16-bit mono audio

            var request = new StartStreamTranscriptionRequest
            {
                LanguageCode = languageCode,
                MediaEncoding = MediaEncoding.Pcm,
                MediaSampleRateHertz = 16000
            };

            // AudioStreamPublisher callback: SDK calls this repeatedly to pull audio chunks
            request.AudioStreamPublisher += async () =>
            {
                if (cancellationToken.IsCancellationRequested || sendOffset >= audioData.Length)
                    return null; // Signal end of stream

                var length = Math.Min(chunkSize, audioData.Length - sendOffset);
                var chunk = new byte[length];
                Array.Copy(audioData, sendOffset, chunk, 0, length);
                sendOffset += length;

                return new global::Amazon.TranscribeStreaming.Model.AudioEvent
                {
                    AudioChunk = new MemoryStream(chunk)
                };
            };

            var results = new List<string>();

            using var response = await transcribeClient.StartStreamTranscriptionAsync(request);

            // Subscribe to transcript events
            response.TranscriptResultStream.TranscriptEventReceived += (sender, e) =>
            {
                foreach (var result in e.EventStreamEvent.Transcript.Results)
                {
                    if (result.IsPartial == false && result.Alternatives.Count > 0)
                    {
                        var transcript = result.Alternatives[0].Transcript;
                        if (!string.IsNullOrEmpty(transcript))
                        {
                            results.Add(transcript);
                        }
                    }
                }
            };

            response.TranscriptResultStream.ExceptionReceived += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Transcribe error: {e.EventStreamException.Message}");
            };

            // Process the event stream (blocks until complete)
            await response.TranscriptResultStream.StartProcessingAsync();

            return string.Join(" ", results);
        }

        internal static string EscapeXml(string text)
        {
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        public void Dispose()
        {
            _pollyClient.Dispose();
        }
    }

    public class AmazonVoiceInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Gender { get; set; } = "";
        public string LanguageCode { get; set; } = "";
        public string LanguageName { get; set; } = "";
        public List<string> SupportedEngines { get; set; } = new();
    }
}
