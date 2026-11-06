using System.Net;
using System.Net.Http;
using AlfredApiWrapper.Domains.File.Requests;
using AlfredApiWrapperTests.TestHelpers;
using Newtonsoft.Json.Linq;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.File.Requests;

namespace AlfredApiWrapperTests.Domains;

public class FileDomainTests
{
    private static readonly Guid FileId = Guid.Parse("eada91d5-ea19-4a80-afbe-166c12649d52");

    // ---- Upload from remote sources ----

    [Fact]
    public async Task UploadAsync_SendsDecomposeChannelAndCorrelation_OmitsUnsetPriority_AndReadsWarnings()
    {
        var jobId = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json(
            $$"""{ "job_id": "{{jobId}}", "correlation_id": "mail:<abc@x>", "warnings": ["The account has no enabled tags."] }""",
            HttpStatusCode.Accepted);
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.UploadAsync(new UploadRequest
        {
            Urls = new List<string> { "https://files.test/a.pdf" },
            Decompose = true,
            ParentFilePrefix = "batch-7",
            PageRotation = 90,
            Channel = "api",
            CorrelationId = "mail:<abc@x>"
        });

        Assert.Equal(jobId, result.JobId);
        Assert.Equal("mail:<abc@x>", result.CorrelationId);
        Assert.Equal("The account has no enabled tags.", Assert.Single(result.Warnings));
        var body = handler.LastRequestBody!;
        Assert.Contains("\"decompose\":true", body);
        Assert.Contains("\"parent_file_prefix\":\"batch-7\"", body);
        Assert.Contains("\"page_rotation\":90", body);
        Assert.Contains("\"channel\":\"api\"", body);
        Assert.Contains("\"correlation_id\":\"mail:<abc@x>\"", body);
        Assert.DoesNotContain("\"priority\"", body);
    }

    // ---- List / events / versions ----

    [Fact]
    public async Task GetAllAsync_SendsPaging_AndMapsFileRows()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        {
          "result": [ {
            "id": "{{FileId}}", "file_name": "SAMPLE.pdf", "status": "finished", "input_type": "derived",
            "extraction_type": "multimodal", "description": "A sample", "page_count": 2, "page_number": 1,
            "metadata": { "customer": "Wayne" }, "effective_metadata": "{\"customer\":\"Wayne\"}",
            "validation_date": "2026-07-01T12:08:39.5Z", "is_data_duplicate": true
          } ],
          "total": 120, "is_empty": false
        }
        """);
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var page = await domain.GetAllAsync(page: 3, pageSize: 50);

        Assert.Equal("https://example.test/api/file/all?currentPage=3&pageSize=50", handler.LastRequest!.RequestUri!.AbsoluteUri);
        Assert.Equal(120, page.Total);
        var file = Assert.Single(page.Result);
        Assert.Equal("multimodal", file.ExtractionType);
        Assert.Equal(2, file.PageCount);
        Assert.Equal("Wayne", file.Metadata!["customer"]!.Value<string>());
        Assert.True(file.IsDataDuplicate);
        Assert.NotNull(file.ValidationDate);
    }

    [Fact]
    public async Task Detail_WithNullMetadata_Deserializes()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{FileId}}", "metadata": null }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var file = await domain.GetAsync(FileId);

        Assert.Equal(FileId, file.Id);
        Assert.True(file.Metadata == null || file.Metadata.Type == JTokenType.Null);
    }

    [Fact]
    public async Task GetEventsAsync_MapsEvents()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        [ { "id": "{{Guid.NewGuid()}}", "file_id": "{{FileId}}", "event_name": "alfred_event_file_done",
            "event_envelope": "{}", "sent_to_event_broker": true, "sent_to_web_hook": false,
            "creation_date": "2026-11-01T10:00:00Z", "update_date": "2026-11-01T10:00:00Z" } ]
        """);
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var events = await domain.GetEventsAsync(FileId);

        Assert.Equal($"https://example.test/api/file/events/{FileId}", handler.LastRequest!.RequestUri!.ToString());
        var e = Assert.Single(events);
        Assert.Equal("alfred_event_file_done", e.EventName);
        Assert.True(e.SentToEventBroker);
        Assert.False(e.SentToWebHook);
    }

    [Fact]
    public async Task GetVersionsAsync_MapsVersions()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        [ { "id": "{{Guid.NewGuid()}}", "file_log_id": "{{Guid.NewGuid()}}", "content": "INVOICE 42", "rotate": 90, "orientation": 1,
            "creation_date": "2026-11-01T10:00:00Z", "update_date": "2026-11-01T10:00:00Z" } ]
        """);
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var versions = await domain.GetVersionsAsync(FileId);

        Assert.Equal($"https://example.test/api/file/versions/{FileId}", handler.LastRequest!.RequestUri!.ToString());
        var v = Assert.Single(versions);
        Assert.Equal("INVOICE 42", v.Content);
        Assert.Equal(90, v.Rotate);
    }

    // ---- Share ----

    [Fact]
    public async Task ShareAsync_PostsSettings_AndReturnsLink()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "file_id": "{{FileId}}", "share_url": "https://tgs.hl/abc" }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.ShareAsync(FileId, new ShareFileRequest { ExpiresInSeconds = 3600, ShortUrl = false });

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal($"https://example.test/api/file/share/{FileId}", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("\"length\":3600", handler.LastRequestBody);
        Assert.Contains("\"short_url\":false", handler.LastRequestBody);
        Assert.Contains("\"always_use_last_version\":true", handler.LastRequestBody);
        Assert.Contains("\"does_not_expire\":false", handler.LastRequestBody);
        Assert.Equal("https://tgs.hl/abc", result.ShareUrl);
    }

    [Fact]
    public async Task ShareAsync_ThatNeverExpires_OmitsLength()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "file_id": "{{FileId}}", "share_url": "u" }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        await domain.ShareAsync(FileId, new ShareFileRequest { DoesNotExpire = true });

        Assert.Contains("\"does_not_expire\":true", handler.LastRequestBody);
        Assert.DoesNotContain("\"length\"", handler.LastRequestBody);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(59)]
    public async Task ShareAsync_WithoutAValidExpiry_IsRejectedBeforeSending(int? seconds)
    {
        var handler = FakeHttpMessageHandler.Json("{}");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        await Assert.ThrowsAsync<ArgumentException>(() => domain.ShareAsync(FileId, new ShareFileRequest { ExpiresInSeconds = seconds }));
        Assert.Null(handler.LastRequest);
    }

    // ---- Reprocess / re-extract / change tag ----

    [Fact]
    public async Task ReprocessAsync_WithDefaults_SendsNoFlags()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{FileId}}", "skip_cache": false, "request_review": false }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.ReprocessAsync(FileId);

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal($"https://example.test/api/file/reprocess/{FileId}", handler.LastRequest.RequestUri!.AbsoluteUri);
        Assert.Equal(FileId, result.Id);
    }

    [Fact]
    public async Task ReprocessAsync_SendsFlags()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{FileId}}", "skip_cache": true, "request_review": true }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.ReprocessAsync(FileId, skipCache: true, requestReview: true);

        Assert.Equal($"https://example.test/api/file/reprocess/{FileId}?skipCache=true&requestReview=true", handler.LastRequest!.RequestUri!.AbsoluteUri);
        Assert.True(result.SkipCache);
        Assert.True(result.RequestReview);
    }

    [Fact]
    public async Task ReprocessAsync_WhenFileIsBusy_ThrowsConflictWithTheServersSentence()
    {
        // The server answers a refused reprocess with a bare JSON string.
        var handler = FakeHttpMessageHandler.Json("\"A run is already processing this file.\"", HttpStatusCode.Conflict);
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var ex = await Assert.ThrowsAsync<ApiException>(() => domain.ReprocessAsync(FileId));

        Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);
        Assert.Equal("A run is already processing this file.", ex.ErrorMessage);
    }

    [Fact]
    public async Task ReextractAsync_SendsRequestReview()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{FileId}}", "request_review": true }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.ReextractAsync(FileId, requestReview: true);

        Assert.Equal($"https://example.test/api/file/reextract/{FileId}?requestReview=true", handler.LastRequest!.RequestUri!.AbsoluteUri);
        Assert.True(result.RequestReview);
        Assert.False(result.SkipCache);
    }

    [Fact]
    public async Task ChangeTagAsync_PostsTagId()
    {
        var tagId = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{FileId}}", "tag_id": "{{tagId}}" }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.ChangeTagAsync(FileId, tagId);

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal($"https://example.test/api/file/{FileId}/tag", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal($$"""{"tag_id":"{{tagId}}"}""", handler.LastRequestBody);
        Assert.Equal(tagId, result.TagId);
    }

    [Fact]
    public async Task ChangeTagAsync_RejectsEmptyTagId()
    {
        var domain = new FileDomain(FakeHttpMessageHandler.Client("{}"));

        await Assert.ThrowsAsync<ArgumentException>(() => domain.ChangeTagAsync(FileId, Guid.Empty));
    }
}
