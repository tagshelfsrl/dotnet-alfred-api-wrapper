using AlfredApiWrapperTests.TestHelpers;
using TagShelf.Alfred.ApiWrapper.Authentication;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Enumerations;
using TagShelf.Alfred.ApiWrapper.Utilities;

namespace AlfredApiWrapperTests.Core;

/// <summary>
/// Tests that mutate the ALFRED_API_KEY process environment variable. Kept in a single class so xUnit
/// runs them sequentially (methods within a class do not run in parallel), avoiding cross-test interference.
/// </summary>
public class EnvironmentApiKeyTests
{
    private const string Var = "ALFRED_API_KEY";

    private static void WithEnv(string? value, Action body)
    {
        var previous = Environment.GetEnvironmentVariable(Var);
        try
        {
            Environment.SetEnvironmentVariable(Var, value);
            body();
        }
        finally
        {
            Environment.SetEnvironmentVariable(Var, previous);
        }
    }

    [Fact]
    public void EnvironmentHelper_ReadsVariable()
    {
        WithEnv("env-value", () =>
            Assert.Equal("env-value", EnvironmentHelper.GetEnvironmentVariable(Var)));
    }

    [Fact]
    public void ApiKeyAuthentication_FromEnvironmentVariable_SetsHeader()
    {
        WithEnv("env-key", () =>
        {
            var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));
            var auth = ApiKeyAuthentication.FromEnvironmentVariable(client);

            Assert.NotNull(auth);
            Assert.Equal("env-key", client.GetDefaultHeaderValue("X-TagshelfAPI-Key"));
        });
    }

    [Fact]
    public void ApiKeyAuthentication_FromEnvironmentVariable_Missing_Throws()
    {
        WithEnv(null, () =>
        {
            var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));
            Assert.Throws<InvalidOperationException>(() => ApiKeyAuthentication.FromEnvironmentVariable(client));
        });
    }

    [Fact]
    public async Task CreateWithApiKeyFromEnvironmentAsync_UsesEnvKey()
    {
        Alfred? alfred = null;
        // Set the env var, create the client, then restore — all synchronously around the await-free factory call.
        var previous = Environment.GetEnvironmentVariable(Var);
        try
        {
            Environment.SetEnvironmentVariable(Var, "env-key");
            alfred = await AlfredFactory.CreateWithApiKeyFromEnvironmentAsync(EnvironmentType.Production);
        }
        finally
        {
            Environment.SetEnvironmentVariable(Var, previous);
        }

        Assert.Equal("env-key", alfred!.ApiClient.GetDefaultHeaderValue("X-TagshelfAPI-Key"));
        Assert.Equal("https://app.tagshelf.com/", alfred.ApiClient.BaseAddress!.ToString());
    }
}
