using System.Net;
using System.Text;
using AlfredApiWrapperTests.TestHelpers;
using TagShelf.Alfred.ApiWrapper.Authentication;
using TagShelf.Alfred.ApiWrapper.Core;

namespace AlfredApiWrapperTests.Authentication;

public class ApiKeyAuthenticationTests
{
    [Fact]
    public void Constructor_SetsApiKeyHeaderOnClient()
    {
        var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));

        _ = new ApiKeyAuthentication("secret-key", client);

        Assert.Equal("secret-key", client.GetDefaultHeaderValue("X-TagshelfAPI-Key"));
    }

    [Fact]
    public void Constructor_RejectsMissingApiKey()
    {
        var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));

        Assert.Throws<ArgumentException>(() => new ApiKeyAuthentication("", client));
        Assert.Throws<ArgumentException>(() => new ApiKeyAuthentication("   ", client));
    }

    [Fact]
    public void Constructor_RejectsNullClient()
    {
        Assert.Throws<ArgumentNullException>(() => new ApiKeyAuthentication("key", null!));
    }

    [Fact]
    public async Task AuthenticateAsync_CompletesImmediately()
    {
        var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));
        var auth = new ApiKeyAuthentication("key", client);

        await auth.AuthenticateAsync(); // should not throw
    }
}

public class OAuthAuthenticationTests
{
    [Fact]
    public void Constructor_RejectsNullArguments()
    {
        var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));

        Assert.Throws<ArgumentNullException>(() => new OAuthAuthentication(null!, "p", client));
        Assert.Throws<ArgumentNullException>(() => new OAuthAuthentication("u", null!, client));
        Assert.Throws<ArgumentNullException>(() => new OAuthAuthentication("u", "p", null!));
    }

    [Fact]
    public async Task AuthenticateAsync_Success_SetsBearerHeader()
    {
        var handler = FakeHttpMessageHandler.Json(
            """{ "access_token": "abc123", "token_type": "bearer", "expires_in": 3600 }""");
        var client = FakeHttpMessageHandler.Client(handler);
        var auth = new OAuthAuthentication("user", "pass", client);

        await auth.AuthenticateAsync();

        Assert.Equal("Bearer abc123", client.GetDefaultHeaderValue("Authorization"));
        Assert.EndsWith("/token", handler.LastRequest!.RequestUri!.ToString());
        Assert.Contains("grant_type=password", handler.LastRequestBody);
        Assert.Contains("username=user", handler.LastRequestBody);
    }

    [Fact]
    public async Task AuthenticateAsync_MalformedResponse_ThrowsInvalidOperation()
    {
        // A 200 response with a non-JSON body makes deserialization throw a JsonException,
        // which OAuthAuthentication wraps as InvalidOperationException.
        var handler = FakeHttpMessageHandler.Json("not-json-at-all");
        var client = FakeHttpMessageHandler.Client(handler);
        var auth = new OAuthAuthentication("user", "pass", client);

        await Assert.ThrowsAsync<InvalidOperationException>(() => auth.AuthenticateAsync());
    }
}

public class HmacAuthenticationTests
{
    // A valid base64 API key is required (it is base64-decoded into the HMAC key).
    private static readonly string ApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("super-secret-bytes"));

    [Fact]
    public void Constructor_RejectsNullArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new HmacAuthentication(null!, ApiKey));
        Assert.Throws<ArgumentNullException>(() => new HmacAuthentication("secret", null!));
    }

    [Fact]
    public void GenerateSignature_WithoutBody_ProducesAmxFormat()
    {
        var hmac = new HmacAuthentication("my-secret", ApiKey);

        var signature = hmac.GenerateSignature("https://app.tagshelf.com/api/thing", "GET");

        Assert.StartsWith("amx ", signature);
        // Format: "amx {secret}:{sig}:{nonce}:{timestamp}"
        var parts = signature.Substring(4).Split(':');
        Assert.Equal(4, parts.Length);
        Assert.Equal("my-secret", parts[0]);
        Assert.NotEmpty(parts[1]);
    }

    [Fact]
    public void GenerateSignature_WithBody_DiffersFromEmptyBody()
    {
        var hmac = new HmacAuthentication("my-secret", ApiKey);

        var withBody = hmac.GenerateSignature("https://app.tagshelf.com/api/thing", "POST", """{ "a": 1 }""");
        var withoutBody = hmac.GenerateSignature("https://app.tagshelf.com/api/thing", "POST");

        Assert.StartsWith("amx my-secret:", withBody);
        Assert.NotEqual(withBody, withoutBody);
    }
}
