using System.Net;
using System.Net.Http;
using AlfredApiWrapperTests.TestHelpers;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Job;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace AlfredApiWrapperTests.Domains;

public class JobDomainTests
{
    private static readonly Guid JobId = Guid.Parse("78dd6181-00aa-4ded-aeaf-57803bbddae3");

    // ---- Detail ----

    [Fact]
    public async Task GetAsync_WithoutExclude_SendsNoQuery_AndMapsNewFields()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        {
          "id": "{{JobId}}", "correlation_id": "erp:INV-1042", "priority": "High",
          "total_file_count": 3, "cancel_count": 1, "restart_count": 2,
          "last_cancellation_date": "2026-11-01T10:00:00Z", "restarted_at": "2026-11-02T10:00:00Z",
          "remote_sources_available": true, "unavailable_source_urls": [],
          "has_failed_files": true, "has_broken_input_sources": false, "stage": "completed"
        }
        """);
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var job = await domain.GetAsync(JobId);

        Assert.Equal($"https://example.test/api/job/detail/{JobId}", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("erp:INV-1042", job.CorrelationId);
        Assert.Equal("High", job.Priority);
        Assert.Equal(3, job.TotalFileCount);
        Assert.Equal(1, job.CancelCount);
        Assert.Equal(2, job.RestartCount);
        Assert.NotNull(job.LastCancellationDate);
        Assert.NotNull(job.RestartedAt);
        Assert.True(job.RemoteSourcesAvailable);
        Assert.True(job.HasFailedFiles);
    }

    [Fact]
    public async Task GetAsync_WithExclude_SendsCommaSeparatedSections()
    {
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{JobId}}" }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        await domain.GetAsync(JobId, JobDetailExclude.Files | JobDetailExclude.ErrorMessages);

        Assert.EndsWith($"api/job/detail/{JobId}?exclude=files%2Cerrormessages", handler.LastRequest!.RequestUri!.AbsoluteUri);
    }

    [Theory]
    [InlineData(JobDetailExclude.None, null)]
    [InlineData(JobDetailExclude.Metadata, "metadata")]
    [InlineData(JobDetailExclude.FileUrls, "fileurls")]
    [InlineData(JobDetailExclude.All, "files,metadata,fileurls,errormessages")]
    public void BuildExcludeValue_MapsFlags(JobDetailExclude exclude, string? expected)
    {
        Assert.Equal(expected, JobDomain.BuildExcludeValue(exclude));
    }

    // ---- List ----

    [Fact]
    public async Task GetAllAsync_WithDefaults_SendsFirstPageNewestFirst()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "result": [], "total": 0, "is_empty": true }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var page = await domain.GetAllAsync();

        Assert.Equal("https://example.test/api/job/all?currentPage=1&pageSize=10&sortDirection=DESC", handler.LastRequest!.RequestUri!.AbsoluteUri);
        Assert.Empty(page.Result);
    }

    [Fact]
    public async Task GetAllAsync_SendsFilters_AndMapsItems()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        {
          "result": [ {
            "id": "{{JobId}}", "creation_date": "2026-11-01T10:00:00Z", "update_date": "2026-11-01T10:05:00Z",
            "user_name": "MagicianApp", "channel": "api", "correlation_id": "erp:INV-1042",
            "merge": false, "decompose": true, "propagate_metadata": false, "file_sources_count": 1,
            "file_count": 4, "finished_files": 4, "retries": 0, "metadata": "{\"k\":\"v\"}",
            "exceeded_retries": false, "stage": "completed", "priority": "normal",
            "has_confirmation_required_files": true, "confirmation_required_files_count": 2,
            "has_failed_files": false, "failed_files_count": 0
          } ],
          "total": 1, "is_empty": false
        }
        """);
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var page = await domain.GetAllAsync(new JobListRequest
        {
            Page = 2,
            PageSize = 25,
            SortDirection = SortDirection.Ascending,
            StartDate = new DateTime(2026, 10, 1, 15, 30, 0),
            EndDate = new DateTime(2026, 10, 31),
            CorrelationId = "erp:INV-1042",
            Channel = "api"
        });

        var uri = handler.LastRequest!.RequestUri!.AbsoluteUri;
        Assert.Contains("currentPage=2", uri);
        Assert.Contains("pageSize=25", uri);
        Assert.Contains("sortDirection=ASC", uri);
        Assert.Contains("startDate=2026-10-01", uri);
        Assert.Contains("endDate=2026-10-31", uri);
        Assert.Contains("correlation_id=erp%3AINV-1042", uri);
        Assert.Contains("channel=api", uri);

        Assert.Equal(1, page.Total);
        var job = Assert.Single(page.Result);
        Assert.Equal(JobId, job.Id);
        Assert.Equal("erp:INV-1042", job.CorrelationId);
        Assert.Equal("completed", job.Stage);
        Assert.Equal(2, job.ConfirmationRequiredFilesCount);
        Assert.Equal("{\"k\":\"v\"}", job.Metadata);
    }

    // ---- Restart / cancel / check sources ----

    [Fact]
    public async Task RestartAsync_PostsJobIdInQuery_AndOmitsFalseFlag()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "restarted": true }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.RestartAsync(JobId);

        Assert.True(result.Restarted);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal($"https://example.test/api/job/restart?jobId={JobId}", handler.LastRequest.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task RestartAsync_WithRedownload_SendsFlag()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "restarted": true }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        await domain.RestartAsync(JobId, redownloadRemoteFiles: true);

        Assert.Contains("redownloadRemoteFiles=true", handler.LastRequest!.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task RestartAsync_RefusedForMissingSources_ThrowsWithReason()
    {
        var handler = FakeHttpMessageHandler.Json(
            """{ "restarted": false, "refused": true, "reason": "sources-missing", "message": "2 sources are missing.", "missing_sources": ["a", "b"] }""",
            HttpStatusCode.Conflict);
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var ex = await Assert.ThrowsAsync<ApiException>(() => domain.RestartAsync(JobId, redownloadRemoteFiles: true));

        Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);
        Assert.Equal("2 sources are missing.", ex.ErrorMessage);
        Assert.Contains("sources-missing", ex.ResponseContent);
    }

    [Fact]
    public async Task CancelAsync_PostsJobIdInQuery()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "canceled": true }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.CancelAsync(JobId);

        Assert.True(result.Canceled);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal($"https://example.test/api/job/cancel?jobId={JobId}", handler.LastRequest.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task CheckSourcesAsync_MapsUnavailableUrls()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "available": false, "unavailable_urls": ["https://gone.test/a.pdf"] }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.CheckSourcesAsync(JobId);

        Assert.False(result.Available);
        Assert.Equal("https://gone.test/a.pdf", Assert.Single(result.UnavailableUrls));
        Assert.Equal($"https://example.test/api/job/check-sources?jobId={JobId}", handler.LastRequest!.RequestUri!.AbsoluteUri);
    }

    // ---- Bulk file operation ----

    [Fact]
    public async Task BulkFileOperationAsync_PostsSnakeCaseBody_AndMapsOutcomes()
    {
        var fileA = Guid.NewGuid();
        var fileB = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""
        { "results": [
          { "id": "{{fileA}}", "outcome": "done", "reason": "" },
          { "id": "{{fileB}}", "outcome": "skipped", "reason": "File does not belong to this job." }
        ] }
        """);
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.BulkFileOperationAsync(JobId, new BulkFileOperationRequest
        {
            FileIds = { fileA, fileB },
            Operation = FileOperation.Reextract,
            RequestReview = true
        });

        Assert.Equal($"https://example.test/api/job/bulk-file-operation?jobId={JobId}", handler.LastRequest!.RequestUri!.AbsoluteUri);
        Assert.Contains("\"file_ids\":[", handler.LastRequestBody);
        Assert.Contains(fileA.ToString(), handler.LastRequestBody);
        Assert.Contains("\"op\":\"reextract\"", handler.LastRequestBody);
        Assert.Contains("\"request_review\":true", handler.LastRequestBody);
        Assert.Equal(2, result.Results.Count);
        Assert.Equal("skipped", result.Results[1].Outcome);
        Assert.Equal("File does not belong to this job.", result.Results[1].Reason);
    }

    [Fact]
    public async Task BulkFileOperationAsync_DefaultsToReprocess()
    {
        var handler = FakeHttpMessageHandler.Json("""{ "results": [] }""");
        var domain = new JobDomain(FakeHttpMessageHandler.Client(handler));

        await domain.BulkFileOperationAsync(JobId, new BulkFileOperationRequest { FileIds = { Guid.NewGuid() } });

        Assert.Contains("\"op\":\"reprocess\"", handler.LastRequestBody);
        Assert.Contains("\"request_review\":false", handler.LastRequestBody);
    }

    [Fact]
    public async Task BulkFileOperationAsync_RejectsNullRequest()
    {
        var domain = new JobDomain(FakeHttpMessageHandler.Client("{}"));

        await Assert.ThrowsAsync<ArgumentNullException>(() => domain.BulkFileOperationAsync(JobId, null!));
    }
}
