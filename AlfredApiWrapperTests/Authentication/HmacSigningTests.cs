using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AlfredApiWrapperTests.TestHelpers;
using TagShelf.Alfred.ApiWrapper.Authentication;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.File.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Job;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Enumerations;
using UploadFileRequest = AlfredApiWrapper.Domains.File.Requests.UploadFileRequest;

namespace AlfredApiWrapperTests.Authentication;

/// <summary>
/// Signs real SDK requests and checks each signature with a port of the server's verification
/// (TagShelf.Web HMACAuthenticationAttribute.isValidRequest), so the SDK and the server agree on
/// what is signed.
/// </summary>
public class HmacSigningTests
{
    private const string SecretKey = "user-secret-key";
    private static readonly string ApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("shared-hmac-key-bytes"));

    /// <summary>A handler that records the request and the exact body bytes the server would receive.</summary>
    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public byte[] Body { get; private set; } = Array.Empty<byte>();
        private readonly string _json;

        public CapturingHandler(string json) => _json = json;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = request.Content == null ? Array.Empty<byte>() : await request.Content.ReadAsByteArrayAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(_json, Encoding.UTF8, "application/json") };
        }
    }

    private static HttpApiClient SignedClient(CapturingHandler handler)
    {
        var client = new HttpApiClient(handler);
        client.SetBaseAddress("https://app.tagshelf.com");
        client.RequestSigner = new HmacAuthentication(SecretKey, ApiKey).SignAsync;
        return client;
    }

    /// <summary>The server's check, line for line: same encoding, same field order, same body hash rule.</summary>
    private static bool ServerAccepts(HttpRequestMessage request, byte[] body)
    {
        var auth = request.Headers.Authorization;
        if (auth == null || !string.Equals(auth.Scheme, "amx", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        var parts = auth.Parameter!.Split(':');
        if (parts.Length != 4)
        {
            return false;
        }
        var (secretKey, incomingSignature, nonce, timestamp) = (parts[0], parts[1], parts[2], parts[3]);

        var requestUri = HttpUtility.UrlEncode(request.RequestUri!.AbsoluteUri.ToLower());
        var bodyHash = body.Length == 0 ? "" : Convert.ToBase64String(MD5.HashData(body));
        var data = $"{secretKey}{request.Method.Method}{requestUri}{timestamp}{nonce}{bodyHash}";
        using var hmac = new HMACSHA256(Convert.FromBase64String(ApiKey));
        var expected = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
        return secretKey == SecretKey && expected == incomingSignature;
    }

    [Fact]
    public async Task Get_WithEncodedQueryCharacters_IsAccepted()
    {
        var handler = new CapturingHandler("""{ "result": [], "total": 0 }""");
        var jobs = new JobDomain(SignedClient(handler));

        // Characters where URL encoders disagree: space, '~', '!', '*', '(', ')', ':' and '/'.
        await jobs.GetAllAsync(new JobListRequest { CorrelationId = "Erp:Inv (1)!*~/x", Channel = "api" });

        Assert.True(handler.Request!.RequestUri!.IsAbsoluteUri);
        Assert.True(ServerAccepts(handler.Request, handler.Body));
    }

    [Fact]
    public async Task PostWithJsonBody_IsAccepted()
    {
        var handler = new CapturingHandler("""{ "results": [] }""");
        var jobs = new JobDomain(SignedClient(handler));

        await jobs.BulkFileOperationAsync(Guid.NewGuid(), new BulkFileOperationRequest
        {
            FileIds = { Guid.NewGuid() },
            Operation = FileOperation.Reextract
        });

        Assert.NotEmpty(handler.Body);
        Assert.True(ServerAccepts(handler.Request!, handler.Body));
    }

    [Fact]
    public async Task PostWithEmptyBody_IsAccepted()
    {
        var handler = new CapturingHandler("""{ "canceled": true }""");
        var jobs = new JobDomain(SignedClient(handler));

        await jobs.CancelAsync(Guid.NewGuid());

        Assert.True(ServerAccepts(handler.Request!, handler.Body));
    }

    [Fact]
    public async Task MultipartUpload_IsAccepted_AndTheFileStillReachesTheServer()
    {
        var handler = new CapturingHandler($$"""{ "file_id": "{{Guid.NewGuid()}}" }""");
        var files = new FileDomain(SignedClient(handler));

        await files.UploadFileAsync(new UploadFileRequest
        {
            FileStream = new MemoryStream(Encoding.UTF8.GetBytes("%PDF-1.4 file bytes")),
            FileName = "invoice.pdf",
            SessionId = Guid.NewGuid()
        });

        // Hashing reads the body; it must still be sent in full afterwards.
        Assert.Contains("%PDF-1.4 file bytes", Encoding.UTF8.GetString(handler.Body));
        Assert.True(ServerAccepts(handler.Request!, handler.Body));
    }

    [Fact]
    public async Task Download_IsSigned()
    {
        var handler = new CapturingHandler("bytes");
        var files = new FileDomain(SignedClient(handler));

        await files.DownloadAsync(Guid.NewGuid());

        Assert.True(ServerAccepts(handler.Request!, handler.Body));
    }

    [Fact]
    public async Task EachRequest_GetsItsOwnNonce()
    {
        var handler = new CapturingHandler("""{ "canceled": true }""");
        var jobs = new JobDomain(SignedClient(handler));

        await jobs.CancelAsync(Guid.NewGuid());
        var first = handler.Request!.Headers.Authorization!.Parameter!.Split(':')[2];
        await jobs.CancelAsync(Guid.NewGuid());
        var second = handler.Request!.Headers.Authorization!.Parameter!.Split(':')[2];

        Assert.NotEqual(first, second);
    }

    [Fact]
    public async Task CreateWithHmacAsync_SignsRequests()
    {
        var alfred = await AlfredFactory.CreateWithHmacAsync(SecretKey, ApiKey, EnvironmentType.Staging);

        Assert.NotNull(alfred.ApiClient.RequestSigner);
    }

    [Fact]
    public async Task CreateWithHmacAsync_RejectsAKeyThatIsNotBase64()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => AlfredFactory.CreateWithHmacAsync(SecretKey, "not base64!"));
    }

    [Theory]
    [InlineData("https://app.tagshelf.com/api/job/detail/ABC", "https%3a%2f%2fapp.tagshelf.com%2fapi%2fjob%2fdetail%2fabc")]
    [InlineData("https://h.test/a?q=x%20y&n=(1)!*~", null)]
    public void EncodeUri_MatchesTheServersEncoder(string uri, string? expected)
    {
        var serverEncoding = HttpUtility.UrlEncode(uri.ToLower());

        Assert.Equal(serverEncoding, HmacAuthentication.EncodeUri(uri));
        if (expected != null)
        {
            Assert.Equal(expected, HmacAuthentication.EncodeUri(uri));
        }
    }
}
