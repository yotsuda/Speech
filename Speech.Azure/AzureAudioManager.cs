using System.Text;
using System.Text.Json;

namespace Speech.Azure
{
    /// <summary>
    /// Manages Azure Speech Services using REST API
    /// </summary>
    public class AzureAudioManager
    {
        private static AzureAudioManager? _instance;
        private static readonly object _lock = new object();

        private readonly string _key;
        private readonly string _region;
        private readonly HttpClient _httpClient;

        private AzureAudioManager(string key, string region)
        {
            _key = key;
            _region = region;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Speech");
        }

        public static AzureAudioManager GetInstance(string key, string region)
        {
            if (_instance == null || _instance._key != key || _instance._region != region)
            {
                lock (_lock)
                {
                    if (_instance == null || _instance._key != key || _instance._region != region)
                    {
                        _instance?.Cleanup();
                        _instance = new AzureAudioManager(key, region);
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// Synthesize speech from SSML using Azure TTS API
        /// </summary>
        public async Task SynthesizeSpeechAsync(string ssml, int deviceNumber = -1)
        {
            var endpoint = $"https://{_region}.tts.speech.microsoft.com/cognitiveservices/v1";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            request.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-128kbitrate-mono-mp3");
            request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody;
                try
                {
                    errorBody = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    errorBody = $"(unable to read response: {ex.Message})";
                }

                // Debug: Save SSML to temp file for troubleshooting
                string debugPath = "";
                try
                {
                    debugPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Voice-debug-{DateTime.Now:yyyyMMdd-HHmmss}.xml");
                    System.IO.File.WriteAllText(debugPath, ssml);
                }
                catch { }

                var errorDetails = $"Azure TTS API error ({response.StatusCode}): {errorBody}\nEndpoint: {endpoint}\nRegion: {_region}\nSSML Length: {ssml.Length}";
                if (!string.IsNullOrEmpty(debugPath))
                {
                    errorDetails += $"\nSSML saved to: {debugPath}";
                }
                throw new Exception(errorDetails);
            }

            var audioData = await response.Content.ReadAsByteArrayAsync();

            // Play audio using NAudio
            await PlayAudioAsync(audioData, deviceNumber);
        }

        /// <summary>
        /// Get available voices from Azure
        /// </summary>
        public async Task<List<AzureSpeechInfo>> GetAvailableVoicesAsync()
        {
            var endpoint = $"https://{_region}.tts.speech.microsoft.com/cognitiveservices/voices/list";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _key);


            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Azure Voices API error ({response.StatusCode}): {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var voices = JsonSerializer.Deserialize<List<AzureSpeechInfo>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return voices ?? new List<AzureSpeechInfo>();
        }

        private async Task PlayAudioAsync(byte[] audioData, int deviceNumber = -1)
        {
            // Validate audio data before attempting playback
            if (audioData == null || audioData.Length == 0)
            {
                throw new InvalidOperationException(
                    "Azure Speech API returned empty audio data. " +
                    "This may indicate an authentication error or invalid request.");
            }

            // Check if this looks like an error response (JSON/XML instead of MP3)
            if (audioData.Length > 0 && (audioData[0] == '{' || audioData[0] == '<'))
            {
                var responseText = System.Text.Encoding.UTF8.GetString(audioData);
                throw new InvalidOperationException(
                    $"Azure Speech API returned an error response instead of audio:\n{responseText}");
            }

            try
            {
                // Use NAudio to play MP3 directly in memory (no media player needed)
                await Task.Run(() =>
                {
                    using (var ms = new System.IO.MemoryStream(audioData))
                    using (var reader = new NAudio.Wave.Mp3FileReader(ms))
                    using (var waveOut = new NAudio.Wave.WaveOutEvent())
                    {
                        waveOut.DeviceNumber = deviceNumber;
                        waveOut.Init(reader);
                        waveOut.Play();

                        // Wait for playback to complete
                        while (waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                });
            }
            catch (System.IO.InvalidDataException ex) when (ex.Message.Contains("MP3"))
            {
                throw new InvalidOperationException(
                    "Invalid audio format from Azure Speech API. " +
                    "This often indicates voice/text language mismatch or invalid voice name.",
                    ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Audio playback failed: {ex.Message}\n\n" +
                    "This may indicate:\n" +
                    "  • Audio device issues\n" +
                    "  • Invalid audio format from Azure API\n" +
                    "  • System configuration problems",
                    ex);
            }
        }
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }
    }

    public class AzureSpeechInfo
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? LocalName { get; set; }
        public string? ShortName { get; set; }
        public string? Gender { get; set; }
        public string? Locale { get; set; }
        public string? LocaleName { get; set; }
        public string? SampleRateHertz { get; set; }
        public string? VoiceType { get; set; }
        public string? Status { get; set; }
        public List<string>? StyleList { get; set; }
        public List<string>? RolePlayList { get; set; }
        public List<string>? SecondaryLocaleList { get; set; }
        public VoiceTag? VoiceTag { get; set; }
        public string? WordsPerMinute { get; set; }
    }

    public class VoiceTag
    {
        public List<string>? TailoredScenarios { get; set; }
        public List<string>? VoicePersonalities { get; set; }
    }
}
