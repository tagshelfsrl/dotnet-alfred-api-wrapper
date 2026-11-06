using System.Net;
using System.Net.Http;
using AlfredApiWrapperTests.TestHelpers;
using Newtonsoft.Json.Linq;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.RemoteSource;
using TagShelf.Alfred.ApiWrapper.Domains.RemoteSource.Requests;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace AlfredApiWrapperTests.Domains;

public class RemoteSourceDomainTests
{
    [Fact]
    public async Task GetAllAsync_MapsSources()
    {
        var handler = FakeHttpMessageHandler.Json($$"""[ { "id": "{{Guid.NewGuid()}}", "source_name": "invoices-bucket" } ]""");
        var domain = new RemoteSourceDomain(FakeHttpMessageHandler.Client(handler));

        var sources = await domain.GetAllAsync();

        Assert.Equal("https://example.test/api/source/list", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("invoices-bucket", Assert.Single(sources).SourceName);
    }

    [Theory]
    [InlineData(RemoteSourceType.Aws, "aws")]
    [InlineData(RemoteSourceType.DigitalOcean, "digital_ocean")]
    [InlineData(RemoteSourceType.Azure, "azure")]
    public async Task CreateAsync_PostsSnakeCaseBody_WithProviderWireName(RemoteSourceType type, string wire)
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{id}}" }""", HttpStatusCode.Accepted);
        var domain = new RemoteSourceDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.CreateAsync(new CreateRemoteSourceRequest
        {
            SourceName = "invoices-bucket",
            Type = type,
            AccountName = "acme",
            AccessKey = "AKIA",
            SecretKey = "s3cr3t",
            Region = "us-east-1"
        });

        Assert.Equal(id, result.Id);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("https://example.test/api/source/create", handler.LastRequest.RequestUri!.ToString());
        var body = JObject.Parse(handler.LastRequestBody!);
        Assert.Equal("invoices-bucket", body["source_name"]!.Value<string>());
        Assert.Equal(wire, body["type"]!.Value<string>());
        Assert.Equal("acme", body["account_name"]!.Value<string>());
        Assert.Equal("us-east-1", body["region"]!.Value<string>());
    }

    [Fact]
    public void CreateRequest_ToString_LeavesCredentialsOut()
    {
        var request = new CreateRemoteSourceRequest { SourceName = "s", SecretKey = "s3cr3t", AccessKey = "AKIA" };

        Assert.DoesNotContain("s3cr3t", request.ToString());
        Assert.DoesNotContain("AKIA", request.ToString());
    }

    [Fact]
    public async Task RemoveAsync_DeletesByEscapedName_AndAcceptsEmptyBody()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var domain = new RemoteSourceDomain(FakeHttpMessageHandler.Client(handler));

        await domain.RemoveAsync("my source");

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Equal("https://example.test/api/source/remove/my%20source", handler.LastRequest.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task RemoveAsync_UnknownName_ThrowsNotFound()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "message": "Not found." }""", HttpStatusCode.NotFound);
        var domain = new RemoteSourceDomain(FakeHttpMessageHandler.Client(handler));

        var ex = await Assert.ThrowsAsync<ApiException>(() => domain.RemoveAsync("nope"));

        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal("Not found.", ex.ErrorMessage);
    }

    [Fact]
    public async Task Requests_AreValidatedBeforeSending()
    {
        var domain = new RemoteSourceDomain(FakeHttpMessageHandler.Client("{}"));

        await Assert.ThrowsAsync<ArgumentNullException>(() => domain.CreateAsync(null!));
        await Assert.ThrowsAsync<ArgumentException>(() => domain.CreateAsync(new CreateRemoteSourceRequest()));
        await Assert.ThrowsAsync<ArgumentException>(() => domain.RemoveAsync(""));
    }
}
