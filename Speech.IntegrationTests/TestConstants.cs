namespace Speech.IntegrationTests
{
    public static class TestConstants
    {
        // Environment variable names
        public const string OpenAIKeyEnv = "SPEECH_TEST_OPENAI_KEY";
        public const string AzureKeyEnv = "SPEECH_TEST_AZURE_KEY";
        public const string AzureRegionEnv = "SPEECH_TEST_AZURE_REGION";
        public const string GoogleCredentialEnv = "SPEECH_TEST_GOOGLE_CREDENTIAL";

        // Default values
        public const string DefaultAzureRegion = "eastus";

        // Test phrases
        public const string HelloWorld = "Hello world";
        public const string HelloWorldSsml = "<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice name='en-US-JennyNeural'>Hello world</voice></speak>";

        // Expected keywords for fuzzy matching
        public static readonly string[] HelloWorldKeywords = { "hello", "world" };

        // Provider defaults
        public const string OpenAIVoice = "alloy";
        public const string OpenAIModel = "tts-1";
        public const string GoogleVoice = "en-US-Wavenet-D";
        public const string GoogleLanguage = "en-US";
    }
}
