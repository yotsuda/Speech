using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Speech.OpenAI
{
    /// <summary>
    /// Manages OpenAI Audio API calls for TTS and STT
    /// </summary>
    public class OpenAIAudioManager : IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.openai.com/v1";

        public static readonly string[] AvailableVoices = new[]
        {
            "alloy", "ash", "ballad", "coral", "echo", "fable",
            "onyx", "nova", "sage", "shimmer", "verse"
        };

        public static readonly string[] AvailableModels = new[]
        {
            "tts-1", "tts-1-hd", "gpt-4o-mini-tts"
        };

        public OpenAIAudioManager(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        /// <summary>
        /// Converts text to speech and returns audio bytes (MP3)
        /// </summary>
        public async Task<byte[]> TextToSpeechAsync(
            string text,
            string voice = "alloy",
            string model = "tts-1",
            double speed = 1.0)
        {
            var requestBody = new
            {
                model = model,
                input = text,
                voice = voice,
                speed = speed
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/audio/speech", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI TTS API error: {response.StatusCode} - {errorBody}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Transcribes audio file to text using Whisper
        /// </summary>
        public async Task<string> SpeechToTextAsync(
            byte[] audioData,
            string fileName = "audio.wav",
            string model = "whisper-1",
            string? language = null)
        {
            using var formContent = new MultipartFormDataContent();

            var audioContent = new ByteArrayContent(audioData);
            audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
            formContent.Add(audioContent, "file", fileName);
            formContent.Add(new StringContent(model), "model");

            if (!string.IsNullOrEmpty(language))
            {
                var langCode = language.Contains('-') ? language.Split('-')[0] : language;
                formContent.Add(new StringContent(langCode), "language");
            }

            var response = await _httpClient.PostAsync($"{BaseUrl}/audio/transcriptions", formContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI Whisper API error: {response.StatusCode} - {errorBody}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            return doc.RootElement.GetProperty("text").GetString() ?? "";
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
