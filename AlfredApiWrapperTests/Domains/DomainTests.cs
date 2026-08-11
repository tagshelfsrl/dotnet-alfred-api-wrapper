using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AlfredApiWrapper.Domains.File.Requests;
using AlfredApiWrapperTests.TestHelpers;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains;
using TagShelf.Alfred.ApiWrapper.Domains.DataPoint;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession;
using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.Job;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Requests;

namespace AlfredApiWrapperTests.Domains;

public class DomainTests
{
    // ---- Tagshelf ----

    [Fact]
    public async Task Tagshelf_StatusAsync_HitsStatusEndpoint()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "api_version": "1.2.3", "db_status": "ok" }""");
        var domain = new TagshelfDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.StatusAsync();

        Assert.Equal("1.2.3", result.ApiVersion);
        Assert.Equal("ok", result.DbStatus);
        Assert.EndsWith("/api/tagshelf/status", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task Tagshelf_WhoAmIAsync_ReturnsIdentity()
    {
        var handler = FakeHttpMessageHandler.Json(
            """{ "user_name": "raul", "company_id": "c1", "roles": ["admin"] }""");
        var domain = new TagshelfDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.WhoAmIAsync();

        Assert.Equal("raul", result.UserName);
        Assert.Equal("c1", result.CompanyId);
        Assert.EndsWith("/api/tagshelf/who-am-i", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task Tagshelf_PingAsync_ReturnsPong()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "pong": "raul@tagshelf.com" }""");
        var domain = new TagshelfDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.PingAsync();

        Assert.Equal("raul@tagshelf.com", result.Pong);
        Assert.EndsWith("/api/tagshelf/ping", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public void Tagshelf_Constructor_RejectsNullClient()
    {
        Assert.Throws<ArgumentNullException>(() => new TagshelfDomain(null!));
    }

    // ---- Job ----

    [Fact]
    public async Task Job_CreateAsync_PostsRequestAndOmitsNulls()
    {
        var handler = FakeHttpMessageHandler.Json(
            $$"""{ "job_id": "{{Guid.NewGuid()}}" }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));
        var request = new CreateJobRequest { SessionId = Guid.NewGuid() };

        var result = await domain.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, result.JobId);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.EndsWith("api/job/create", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("session_id", handler.LastRequestBody);
        Assert.Contains("\"priority\":\"normal\"", handler.LastRequestBody);
        // Null-valued optional properties must be omitted.
        Assert.DoesNotContain("\"merge\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task Job_GetAsync_HitsDetailEndpoint()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json(
            $$"""{ "id": "{{id}}", "user_name": "raul", "stage": "classification" }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetAsync(id);

        Assert.Equal(id, result.Id);
        Assert.Equal("classification", result.Stage);
        Assert.EndsWith($"api/job/detail/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    // ---- File ----

    [Fact]
    public async Task File_GetAsync_ReturnsFileDetail()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json(
            $$"""{ "id": "{{id}}", "file_name": "doc.pdf", "status": "finished", "file_size": 2048 }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetAsync(id);

        Assert.Equal("doc.pdf", result.FileName);
        Assert.Equal(2048, result.FileSize);
        Assert.EndsWith($"api/file/detail/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task File_DownloadAsync_ReadsContentTypeNameAndStream()
    {
        var handler = new FakeHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes("PDFDATA"))
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "\"invoice.pdf\""
            };
            return response;
        });
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.DownloadAsync(Guid.NewGuid());

        Assert.Equal("application/pdf", result.ContentType);
        Assert.Equal("invoice.pdf", result.FileName);
        using var reader = new StreamReader(result.FileContents);
        Assert.Equal("PDFDATA", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task File_UploadAsync_PostsUrlRequest()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "job_id": "{{Guid.NewGuid()}}" }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));
        var request = new UploadRequest { Url = "https://files.test/a.pdf", Source = "web" };

        var result = await domain.UploadAsync(request);

        Assert.NotEqual(Guid.Empty, result.JobId);
        Assert.EndsWith("api/file/upload", handler.LastRequest!.RequestUri!.ToString());
        Assert.Contains("https://files.test/a.pdf", handler.LastRequestBody);
    }

    [Fact]
    public async Task File_UploadFileAsync_InfersContentTypeFromExtension()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "file_id": "{{id}}" }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));
        var request = new UploadFileRequest
        {
            FileStream = new MemoryStream(Encoding.UTF8.GetBytes("data")),
            FileName = "report.pdf",
            SessionId = Guid.NewGuid(),
            Metadata = """{ "k": "v" }"""
        };

        var result = await domain.UploadFileAsync(request);

        Assert.Equal(id, result.FileId);
        Assert.EndsWith("/api/file/uploadfile", handler.LastRequest!.RequestUri!.ToString());
        var mediaType = handler.LastRequest.Content as MultipartFormDataContent;
        Assert.NotNull(mediaType);
        Assert.Contains("application/pdf", handler.LastRequestBody);
    }

    [Fact]
    public async Task File_UploadFileAsync_UsesExplicitContentType_WhenProvided()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "file_id": "{{Guid.NewGuid()}}" }""");
        var domain = new FileDomain(FakeHttpMessageHandler.Client(handler));
        var request = new UploadFileRequest
        {
            FileStream = new MemoryStream(new byte[] { 1, 2 }),
            FileName = "file.unknownext",
            SessionId = Guid.NewGuid(),
            ContentType = "application/x-custom"
        };

        await domain.UploadFileAsync(request);

        Assert.Contains("application/x-custom", handler.LastRequestBody);
    }

    // ---- Account ----

    [Fact]
    public async Task Account_SetupWebhookAsync_PostsUrl()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{id}}" }""");
        var domain = new AccountDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.SetupWebhookAsync(new WebhookSetupRequest { Url = "https://hook.test/x" });

        Assert.Equal(id, result.Id);
        Assert.EndsWith("/api/account/webhook", handler.LastRequest!.RequestUri!.ToString());
        Assert.Contains("https://hook.test/x", handler.LastRequestBody);
    }

    // ---- DataPoint ----

    [Fact]
    public async Task DataPoint_GetValuesAsync_ReturnsList_WithFieldKeyAndNormalizedValue()
    {
        var handler = FakeHttpMessageHandler.Json("""
        [ { "metadata_name": "total", "field_key": "total", "value": "100.00",
            "classification_score": 0.9, "normalized_value": "100.00" } ]
        """);
        var domain = new DataPointDomain(FakeHttpMessageHandler.Client(handler));
        var fileId = Guid.NewGuid();

        var result = await domain.GetValuesAsync(fileId);

        Assert.Single(result);
        Assert.Equal("total", result[0].MetadataName);
        Assert.Equal("total", result[0].FieldKey);
        Assert.Equal("100.00", result[0].Value);
        Assert.Equal("100.00", result[0].NormalizedValue);
        // A value without validation leaves the property null.
        Assert.Null(result[0].Validation);
        Assert.EndsWith($"api/values/file/{fileId}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task DataPoint_GetValuesAsync_MapsValidationBlock()
    {
        // Shape mirrors the production payload (snake_case data point, PascalCase validation block).
        var handler = FakeHttpMessageHandler.Json("""
        [
          {
            "metadata_name": "NoCedula", "field_key": "no_cedula", "value": "402-1912052-0",
            "classification_score": 0.01, "normalized_value": "402-1912052-0",
            "validation": {
              "Status": "passed",
              "ExecutedAt": "2026-08-11T17:20:22.9238048Z",
              "DurationMs": 1843,
              "Results": [
                { "ValidatorId": "v_2yy94x", "Type": "required", "Passed": true, "Skipped": false,
                  "Severity": 1, "BlocksCompletion": false, "DurationMs": 0 },
                { "ValidatorId": "v_6snzhu", "Type": "custom", "Passed": true, "Skipped": false,
                  "Severity": 1, "BlocksCompletion": false,
                  "Message": "Cedula validated successfully against padron API.",
                  "Metadata": { "cedula_matches": true, "api_code": 200 },
                  "DurationMs": 1842, "Runtime": "python" }
              ],
              "Summary": { "Total": 2, "Passed": 2, "Failed": 0, "Skipped": 0, "HasBlockingFailures": false },
              "MustPassValidation": true
            }
          }
        ]
        """);
        var domain = new DataPointDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetValuesAsync(Guid.NewGuid());
        var validation = result[0].Validation;

        Assert.NotNull(validation);
        Assert.Equal("passed", validation.Status);
        Assert.Equal(new DateTime(2026, 8, 11, 17, 20, 22, DateTimeKind.Utc), validation.ExecutedAt.ToUniversalTime(), TimeSpan.FromSeconds(1));
        Assert.Equal(1843, validation.DurationMs);
        Assert.True(validation.MustPassValidation);

        Assert.Equal(2, validation.Results.Count);
        var custom = validation.Results.Single(r => r.Type == "custom");
        Assert.True(custom.Passed);
        Assert.Equal("python", custom.Runtime);
        Assert.Contains("padron", custom.Message);
        Assert.NotNull(custom.Metadata);
        Assert.True((bool)custom.Metadata["cedula_matches"]!);
        Assert.Equal(200, (int)custom.Metadata["api_code"]!);

        Assert.Equal(2, validation.Summary.Total);
        Assert.Equal(2, validation.Summary.Passed);
        Assert.False(validation.Summary.HasBlockingFailures);
    }

    // ---- Deferred Session ----

    [Fact]
    public async Task DeferredSession_CreateAsync_PostsCreate()
    {
        var sessionId = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "session_id": "{{sessionId}}" }""");
        var domain = new DeferredSessionDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.CreateAsync();

        Assert.Equal(sessionId, result.SessionId);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.EndsWith("api/deferred/create", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeferredSession_GetAsync_ReturnsDetail()
    {
        var sessionId = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json(
            $$"""{ "id": "{{sessionId}}", "status": "open", "user_name": "raul", "company_id": "c1" }""");
        var domain = new DeferredSessionDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetAsync(sessionId);

        Assert.Equal("open", result.Status);
        Assert.Equal("raul", result.UserName);
        Assert.EndsWith($"api/deferred/detail/{sessionId}", handler.LastRequest!.RequestUri!.ToString());
    }
}
