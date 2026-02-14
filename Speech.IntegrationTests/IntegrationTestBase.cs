namespace Speech.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected static string? GetOpenAIKey() =>
            Environment.GetEnvironmentVariable(TestConstants.OpenAIKeyEnv);

        protected static string? GetAzureKey() =>
            Environment.GetEnvironmentVariable(TestConstants.AzureKeyEnv);

        protected static string GetAzureRegion() =>
            Environment.GetEnvironmentVariable(TestConstants.AzureRegionEnv) ?? TestConstants.DefaultAzureRegion;

        protected static string? GetGoogleCredential() =>
            Environment.GetEnvironmentVariable(TestConstants.GoogleCredentialEnv);

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
