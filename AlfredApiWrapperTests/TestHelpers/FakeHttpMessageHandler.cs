using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TagShelf.Alfred.ApiWrapper.Core;

namespace AlfredApiWrapperTests.TestHelpers;

/// <summary>
/// An <see cref="HttpMessageHandler"/> that returns a canned response and records the request it received,
/// so tests can exercise the SDK's HTTP layer without touching the network.
/// </summary>
public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    /// <summary>The most recent request the handler processed.</summary>
    public HttpRequestMessage? LastRequest { get; private set; }

    /// <summary>The body of the most recent request, read as a string.</summary>
    public string? LastRequestBody { get; private set; }

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        if (request.Content != null)
        {
            LastRequestBody = await request.Content.ReadAsStringAsync();
        }
        return _responder(request);
    }

    /// <summary>Builds a handler that always returns the given status code and JSON body.</summary>
    public static FakeHttpMessageHandler Json(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new FakeHttpMessageHandler(_ => new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }

    /// <summary>Builds an <see cref="HttpApiClient"/> wired to a fake handler returning the given JSON.</summary>
    public static HttpApiClient Client(string json, HttpStatusCode status = HttpStatusCode.OK, string baseAddress = "https://example.test")
    {
        return Client(Json(json, status), baseAddress);
    }

    /// <summary>Builds an <see cref="HttpApiClient"/> wired to the supplied handler.</summary>
    public static HttpApiClient Client(FakeHttpMessageHandler handler, string baseAddress = "https://example.test")
    {
        var client = new HttpApiClient(handler);
        client.SetBaseAddress(baseAddress);
        return client;
    }
}
