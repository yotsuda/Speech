using Speech.Core;
using Speech.Azure;
using Speech.Google;
using Speech.OpenAI;
using Speech.Windows;

namespace Speech.IntegrationTests.Roundtrip
{
    public class CrossProviderTests : IntegrationTestBase
    {
        // Azure TTS → OpenAI STT
        [SkippableFact]
        public async Task Azure_To_OpenAI_Roundtrip()
        {
            Skip.IfNot(HasAzureKey() && HasOpenAIKey(), "Azure + OpenAI keys required");

            var azure = AzureAudioManager.GetInstance(GetAzureKey()!, GetAzureRegion());
            var openai = new OpenAIAudioManager(GetOpenAIKey()!);

            var mp3 = await azure.SynthesizeToByteArrayAsync(TestConstants.HelloWorldSsml);
            var wav = AudioConverter.Mp3ToWav(mp3);
            var result = await openai.SpeechToTextAsync(wav, fileName: "audio.wav", language: "en");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // Azure TTS → Google STT
        [SkippableFact]
        public async Task Azure_To_Google_Roundtrip()
        {
            Skip.IfNot(HasAzureKey() && HasGoogleCredential(), "Azure + Google keys required");

            var azure = AzureAudioManager.GetInstance(GetAzureKey()!, GetAzureRegion());
            var google = new GoogleAudioManager(GetGoogleCredential()!);

            var mp3 = await azure.SynthesizeToByteArrayAsync(TestConstants.HelloWorldSsml);
            var pcm = AudioConverter.Mp3ToPcm(mp3);
            var result = await google.SpeechToTextAsync(pcm, languageCode: TestConstants.GoogleLanguage);

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // OpenAI TTS → Google STT
        [SkippableFact]
        public async Task OpenAI_To_Google_Roundtrip()
        {
            Skip.IfNot(HasOpenAIKey() && HasGoogleCredential(), "OpenAI + Google keys required");

            var openai = new OpenAIAudioManager(GetOpenAIKey()!);
            var google = new GoogleAudioManager(GetGoogleCredential()!);

            var mp3 = await openai.TextToSpeechAsync(TestConstants.HelloWorld, voice: TestConstants.OpenAIVoice);
            var pcm = AudioConverter.Mp3ToPcm(mp3);
            var result = await google.SpeechToTextAsync(pcm, languageCode: TestConstants.GoogleLanguage);

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // OpenAI TTS → Azure STT
        [SkippableFact]
        public async Task OpenAI_To_Azure_Roundtrip()
        {
            Skip.IfNot(HasOpenAIKey() && HasAzureKey(), "OpenAI + Azure keys required");

            var openai = new OpenAIAudioManager(GetOpenAIKey()!);
            var azure = AzureAudioManager.GetInstance(GetAzureKey()!, GetAzureRegion());

            var mp3 = await openai.TextToSpeechAsync(TestConstants.HelloWorld, voice: TestConstants.OpenAIVoice);
            var pcm = AudioConverter.Mp3ToPcm(mp3);
            var result = await azure.RecognizeFromBytesAsync(pcm, language: "en-US");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // Google TTS → OpenAI STT
        [SkippableFact]
        public async Task Google_To_OpenAI_Roundtrip()
        {
            Skip.IfNot(HasGoogleCredential() && HasOpenAIKey(), "Google + OpenAI keys required");

            var google = new GoogleAudioManager(GetGoogleCredential()!);
            var openai = new OpenAIAudioManager(GetOpenAIKey()!);

            var mp3 = await google.TextToSpeechAsync(TestConstants.HelloWorld, voiceName: TestConstants.GoogleVoice, languageCode: TestConstants.GoogleLanguage);
            var wav = AudioConverter.Mp3ToWav(mp3);
            var result = await openai.SpeechToTextAsync(wav, fileName: "audio.wav", language: "en");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // Google TTS → Azure STT
        [SkippableFact]
        public async Task Google_To_Azure_Roundtrip()
        {
            Skip.IfNot(HasGoogleCredential() && HasAzureKey(), "Google + Azure keys required");

            var google = new GoogleAudioManager(GetGoogleCredential()!);
            var azure = AzureAudioManager.GetInstance(GetAzureKey()!, GetAzureRegion());

            var mp3 = await google.TextToSpeechAsync(TestConstants.HelloWorld, voiceName: TestConstants.GoogleVoice, languageCode: TestConstants.GoogleLanguage);
            var pcm = AudioConverter.Mp3ToPcm(mp3);
            var result = await azure.RecognizeFromBytesAsync(pcm, language: "en-US");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // Windows TTS → OpenAI STT
        [SkippableFact]
        public async Task Windows_To_OpenAI_Roundtrip()
        {
            Skip.IfNot(WindowsAudioManager.IsWindowsPlatform() && HasOpenAIKey(), "Windows + OpenAI key required");

            var openai = new OpenAIAudioManager(GetOpenAIKey()!);

            var wav = WindowsAudioManager.SynthesizeToByteArray(TestConstants.HelloWorld);
            // OpenAI Whisper accepts WAV directly
            var result = await openai.SpeechToTextAsync(wav, fileName: "audio.wav", language: "en");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // Windows TTS → Google STT
        [SkippableFact]
        public async Task Windows_To_Google_Roundtrip()
        {
            Skip.IfNot(WindowsAudioManager.IsWindowsPlatform() && HasGoogleCredential(), "Windows + Google key required");

            var google = new GoogleAudioManager(GetGoogleCredential()!);

            var wav = WindowsAudioManager.SynthesizeToByteArray(TestConstants.HelloWorld);
            var pcm = AudioConverter.WavToPcm(wav);
            var result = await google.SpeechToTextAsync(pcm, languageCode: TestConstants.GoogleLanguage);

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }

        // Windows TTS → Azure STT
        [SkippableFact]
        public async Task Windows_To_Azure_Roundtrip()
        {
            Skip.IfNot(WindowsAudioManager.IsWindowsPlatform() && HasAzureKey(), "Windows + Azure key required");

            var azure = AzureAudioManager.GetInstance(GetAzureKey()!, GetAzureRegion());

            var wav = WindowsAudioManager.SynthesizeToByteArray(TestConstants.HelloWorld);
            var pcm = AudioConverter.WavToPcm(wav);
            var result = await azure.RecognizeFromBytesAsync(pcm, language: "en-US");

            AssertContainsKeywords(result, TestConstants.HelloWorldKeywords);
        }
    }
}
