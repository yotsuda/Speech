using Speech.Core;

namespace Speech.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        private static readonly Lazy<SpeechConfig> _config = new(() => ConfigManager.GetConfig());

        protected static string? GetOpenAIKey() =>
            Environment.GetEnvironmentVariable(TestConstants.OpenAIKeyEnv)
            ?? _config.Value.OpenAI?.Key;

        protected static string? GetAzureKey() =>
            Environment.GetEnvironmentVariable(TestConstants.AzureKeyEnv)
            ?? _config.Value.Azure?.Key;

        protected static string GetAzureRegion() =>
            Environment.GetEnvironmentVariable(TestConstants.AzureRegionEnv)
            ?? _config.Value.Azure?.Region
            ?? TestConstants.DefaultAzureRegion;

        protected static string? GetGoogleCredential() =>
            Environment.GetEnvironmentVariable(TestConstants.GoogleCredentialEnv)
            ?? _config.Value.Google?.Credential;

        protected static bool HasOpenAIKey() =>
            !string.IsNullOrEmpty(GetOpenAIKey());

        protected static bool HasAzureKey() =>
            !string.IsNullOrEmpty(GetAzureKey());

        protected static bool HasGoogleCredential() =>
            !string.IsNullOrEmpty(GetGoogleCredential());

        protected static void AssertContainsKeywords(string result, params string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                Assert.Contains(keyword, result, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
