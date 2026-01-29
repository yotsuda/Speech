using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Speech.Google
{
    public class GoogleAudioManager : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleAuthManager _authManager;

        public GoogleAudioManager(string credentialPath)
        {
            _authManager = new GoogleAuthManager(credentialPath);
            _httpClient = new HttpClient();
        }

        private async Task EnsureAuthHeaderAsync()
        {
            var token = await _authManager.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<byte[]> TextToSpeechAsync(string text, string voiceName, string languageCode, double? speakingRate = null, double? pitch = null)
        {
            await EnsureAuthHeaderAsync();

            var url = "https://texttospeech.googleapis.com/v1/text:synthesize";

            var audioConfig = new Dictionary<string, object>
            {
                ["audioEncoding"] = "MP3"
            };
            if (speakingRate.HasValue) audioConfig["speakingRate"] = speakingRate.Value;
            if (pitch.HasValue) audioConfig["pitch"] = pitch.Value;

            var request = new Dictionary<string, object>
            {
                ["input"] = new Dictionary<string, string> { ["text"] = text },
                ["voice"] = new Dictionary<string, string>
                {
                    ["languageCode"] = languageCode,
                    ["name"] = voiceName
                },
                ["audioConfig"] = audioConfig
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Google TTS API error: {response.StatusCode} - {responseBody}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var audioContent = doc.RootElement.GetProperty("audioContent").GetString();

            if (string.IsNullOrEmpty(audioContent))
            {
                throw new InvalidOperationException("No audio content returned from Google TTS API");
            }

            return Convert.FromBase64String(audioContent);
        }

        public async Task<string> SpeechToTextAsync(byte[] audioData, string languageCode, int sampleRateHertz = 16000)
        {
            await EnsureAuthHeaderAsync();

            var url = "https://speech.googleapis.com/v1/speech:recognize";

            var request = new Dictionary<string, object>
            {
                ["config"] = new Dictionary<string, object>
                {
                    ["encoding"] = "LINEAR16",
                    ["sampleRateHertz"] = sampleRateHertz,
                    ["languageCode"] = languageCode,
                    ["enableAutomaticPunctuation"] = true
                },
                ["audio"] = new Dictionary<string, string>
                {
                    ["content"] = Convert.ToBase64String(audioData)
                }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Google STT API error: {response.StatusCode} - {responseBody}");
            }

            using var doc = JsonDocument.Parse(responseBody);

            if (!doc.RootElement.TryGetProperty("results", out var results))
            {
                return "";
            }

            var sb = new StringBuilder();
            foreach (var result in results.EnumerateArray())
            {
                if (result.TryGetProperty("alternatives", out var alternatives))
                {
                    foreach (var alt in alternatives.EnumerateArray())
                    {
                        if (alt.TryGetProperty("transcript", out var transcript))
                        {
                            if (sb.Length > 0) sb.Append(" ");
                            sb.Append(transcript.GetString());
                        }
                        break; // First alternative only
                    }
                }
            }

            return sb.ToString();
        }

        public async Task<List<GoogleVoiceInfo>> GetVoicesAsync(string? languageCode = null)
        {
            await EnsureAuthHeaderAsync();

            var url = "https://texttospeech.googleapis.com/v1/voices";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Google Voices API error: {response.StatusCode} - {responseBody}");
            }

            var voices = new List<GoogleVoiceInfo>();

            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("voices", out var voicesArray))
            {
                foreach (var voice in voicesArray.EnumerateArray())
                {
                    var name = voice.GetProperty("name").GetString() ?? "";
                    var gender = voice.GetProperty("ssmlGender").GetString() ?? "";
                    var langs = voice.GetProperty("languageCodes").EnumerateArray()
                        .Select(l => l.GetString() ?? "").ToList();

                    // Filter by language if specified
                    if (languageCode != null && !langs.Any(l => l.StartsWith(languageCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    voices.Add(new GoogleVoiceInfo
                    {
                        Name = name,
                        Gender = gender,
                        LanguageCodes = langs
                    });
                }
            }

            return voices.OrderBy(v => v.Name).ToList();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }

    public class GoogleVoiceInfo
    {
        public string Name { get; set; } = "";
        public string Gender { get; set; } = "";
        public List<string> LanguageCodes { get; set; } = new();
    }
}
