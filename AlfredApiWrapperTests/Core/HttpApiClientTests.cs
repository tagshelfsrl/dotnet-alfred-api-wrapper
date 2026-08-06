using System.Net;
using System.Net.Http;
using System.Text;
using AlfredApiWrapperTests.TestHelpers;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;

namespace AlfredApiWrapperTests.Core;

public class HttpApiClientTests
{
    private class Sample
    {
        [JsonProperty("value")]
        public string? Value { get; set; }
    }

    [Fact]
    public async Task GetAsync_DeserializesSuccessResponse_AndSendsGet()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "value": "hello" }""");
        var client = FakeHttpMessageHandler.Client(handler);

        var result = await client.GetAsync<Sample>("api/thing");

        Assert.Equal("hello", result.Value);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("https://example.test/api/thing", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task PostAsync_WithContent_SendsBodyAndDeserializes()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "value": "created" }""");
        var client = FakeHttpMessageHandler.Client(handler);
        var content = new StringContent("""{ "name": "x" }""", Encoding.UTF8, "application/json");

        var result = await client.PostAsync<Sample>("api/create", content);

        Assert.Equal("created", result.Value);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Contains("\"name\": \"x\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task PostAsync_WithoutContent_StillSendsJsonBody()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "value": "ok" }""");
        var client = FakeHttpMessageHandler.Client(handler);

        var result = await client.PostAsync<Sample>("api/empty");

        Assert.Equal("ok", result.Value);
        Assert.NotNull(handler.LastRequest!.Content);
        Assert.Equal("application/json", handler.LastRequest.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task SendAsync_NonSuccess_ThrowsApiExceptionWithDetails()
    {
        var handler = FakeHttpMessageHandler.Json(
            """{ "error": "bad_request", "message": "Invalid input" }""",
            HttpStatusCode.BadRequest);
        var client = FakeHttpMessageHandler.Client(handler);

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetAsync<Sample>("api/thing"));

        Assert.Equal("bad_request", ex.Error);
        Assert.Equal("Invalid input", ex.ErrorMessage);
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Contains("Invalid input", ex.ResponseContent);
    }

    [Fact]
    public async Task SendAsync_NetworkError_ThrowsApiExceptionWithInnerException()
    {
        var handler = new FakeHttpMessageHandler(_ => throw new HttpRequestException("connection refused"));
        var client = FakeHttpMessageHandler.Client(handler);

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetAsync<Sample>("api/thing"));

        Assert.Equal("Network error", ex.Error);
        Assert.Null(ex.StatusCode);
        Assert.IsType<HttpRequestException>(ex.InnerException);
    }

    [Fact]
    public async Task GetFileAsync_Success_ReturnsRawResponse()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(new byte[] { 1, 2, 3 })
        });
        var client = FakeHttpMessageHandler.Client(handler);

        var response = await client.GetFileAsync("api/file/download/1");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(3, (await response.Content.ReadAsByteArrayAsync()).Length);
    }

    [Fact]
    public async Task GetFileAsync_NonSuccess_Throws()
    {
        var handler = FakeHttpMessageHandler.Json("nope", HttpStatusCode.NotFound);
        var client = FakeHttpMessageHandler.Client(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetFileAsync("api/file/download/missing"));
    }

    [Fact]
    public void SetBaseAddress_IsExposedViaAccessor()
    {
        var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));
        client.SetBaseAddress("https://staging.tagshelf.com");

        Assert.Equal("https://staging.tagshelf.com/", client.BaseAddress!.ToString());
    }

    [Fact]
    public void AddOrUpdateHeader_AddsThenUpdates()
    {
        var client = new HttpApiClient(FakeHttpMessageHandler.Json("{}"));

        client.AddOrUpdateHeader("X-Test", "first");
        Assert.Equal("first", client.GetDefaultHeaderValue("X-Test"));

        client.AddOrUpdateHeader("X-Test", "second");
        Assert.Equal("second", client.GetDefaultHeaderValue("X-Test"));

        Assert.Null(client.GetDefaultHeaderValue("X-Absent"));
    }

    [Fact]
    public void DefaultConstructor_SetsUserAgent()
    {
        var client = new HttpApiClient();

        Assert.Contains("dotnet-alfred-api-wrapper", client.GetDefaultHeaderValue("User-Agent"));
    }
}
