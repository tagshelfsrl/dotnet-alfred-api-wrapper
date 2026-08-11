using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession.Results;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Results;
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Results;

namespace AlfredApiWrapperTests.Dtos;

/// <summary>
/// Deserializes a fully-populated JSON payload into each result DTO and serializes it back, exercising
/// every property getter and setter (and the ToString override) in one pass.
/// </summary>
public class DtoRoundTripTests
{
    private static T RoundTrip<T>(string json)
    {
        var obj = JsonConvert.DeserializeObject<T>(json)!;
        // Serializing back reads every getter.
        var reserialized = JsonConvert.SerializeObject(obj);
        Assert.False(string.IsNullOrEmpty(reserialized));
        _ = obj!.ToString();
        return obj;
    }

    [Fact]
    public void JobDetailResult_FullRoundTrip()
    {
        var json = """
        {
          "id": "11111111-1111-1111-1111-111111111111",
          "creation_date": "2026-01-01T10:00:00Z",
          "has_job_request_info": true,
          "job_request_date": "2026-01-01T09:00:00Z",
          "update_date": "2026-01-02T10:00:00Z",
          "company_id": "c1", "bulk_id": "b1", "deferred_session_id": "s1",
          "user_name": "raul", "channel": "library", "source": "web", "container": "docs",
          "remote_file_name": "a.pdf", "remote_file_names": ["a.pdf", "b.pdf"],
          "merge": true, "decompose": false, "propagate_metadata": true,
          "parent_file_prefix": "p", "decomposed_page_rotation": 90,
          "file_count": 2, "file_sources_count": 1, "metadata_objects_count": 3, "finished_files": 2,
          "files": [
            { "id": "22222222-2222-2222-2222-222222222222", "creation_date": "2026-01-01T10:00:00Z",
              "update_date": "2026-01-01T11:00:00Z", "tag_name": "invoice", "is_parent": true,
              "is_children": false, "file_name": "a.pdf", "status": "finished" }
          ],
          "retries": 1, "exceeded_retries": false,
          "file_urls": ["https://x/a.pdf"], "error_messages": ["none"],
          "stage": "classification", "start_date": "2026-01-01T10:05:00Z",
          "email_from": "a@b.com", "email_subject": "subj", "email_body": "body"
        }
        """;

        var result = RoundTrip<JobDetailResult>(json);

        Assert.Equal("raul", result.UserName);
        Assert.Single(result.Files);
        Assert.Equal("invoice", result.Files[0].TagName);
        Assert.Equal(2, result.FileCount);
        Assert.True(result.Merge);
    }

    [Fact]
    public void FileDetailResult_FullRoundTrip()
    {
        var json = """
        {
          "id": "33333333-3333-3333-3333-333333333333",
          "creation_date": "2026-01-01T10:00:00Z", "update_date": "2026-01-02T10:00:00Z",
          "file_name": "doc.pdf", "file_name_without_extension": "doc",
          "blob_name": "blob", "blob_url": "https://x/blob", "user_name": "raul",
          "md5_hash": "abc", "content_type": "application/pdf", "channel": "library",
          "should_be_classified": true, "classifier": "auto", "classification_score": 0.95,
          "status": "finished", "is_duplicate": false,
          "duplicate_origin_id": "44444444-4444-4444-4444-444444444444",
          "tag_id": "55555555-5555-5555-5555-555555555555", "is_parent": true,
          "parent_id": "66666666-6666-6666-6666-666666666666",
          "deferred_session_id": "77777777-7777-7777-7777-777777777777",
          "tag_name": "invoice", "company_id": "c1", "file_size": 4096,
          "proposed_tag_id": "88888888-8888-8888-8888-888888888888", "proposed_tag_variance": 0.1,
          "classification_score_above_deviation": true,
          "confirmed_tag_id": "99999999-9999-9999-9999-999999999999",
          "confirmed_by": "raul", "manual_classification": false
        }
        """;

        var result = RoundTrip<FileDetailResult>(json);

        Assert.Equal("doc.pdf", result.FileName);
        Assert.Equal(4096, result.FileSize);
        Assert.Equal(0.95, result.ClassificationScore, precision: 5);
        Assert.Equal("invoice", result.TagName);
    }

    [Fact]
    public void JobFileDetail_FullRoundTrip()
    {
        var json = """
        {
          "id": "22222222-2222-2222-2222-222222222222",
          "creation_date": "2026-01-01T10:00:00Z", "update_date": "2026-01-01T11:00:00Z",
          "tag_name": "invoice", "is_parent": true, "is_children": false,
          "file_name": "a.pdf", "status": "finished"
        }
        """;

        var result = RoundTrip<JobFileDetail>(json);

        Assert.Equal("a.pdf", result.FileName);
        Assert.True(result.IsParent);
        Assert.False(result.IsChildren);
    }

    [Fact]
    public void DataPointResult_FullRoundTrip()
    {
        var json = """
        {
          "id": "11111111-1111-1111-1111-111111111111",
          "file_log_id": "22222222-2222-2222-2222-222222222222",
          "metadata_id": "33333333-3333-3333-3333-333333333333",
          "metadata_name": "no_cedula", "field_key": "no_cedula", "value": "402-1912052-0",
          "classification_score": 0.9, "normalized_value": "402-1912052-0",
          "validation": {
            "Status": "passed",
            "ExecutedAt": "2026-08-11T17:20:22.9238048Z",
            "DurationMs": 1843,
            "Results": [
              { "ValidatorId": "v_2yy94x", "Type": "required", "Passed": true, "Skipped": false,
                "Severity": 1, "BlocksCompletion": false, "DurationMs": 0 },
              { "ValidatorId": "v_6snzhu", "Type": "custom", "Passed": true, "Skipped": false,
                "Severity": 1, "BlocksCompletion": false, "Message": "ok",
                "Metadata": { "api_code": 200 }, "DurationMs": 1842, "Runtime": "python" }
            ],
            "Summary": { "Total": 2, "Passed": 2, "Failed": 0, "Skipped": 0, "HasBlockingFailures": false },
            "MustPassValidation": true
          }
        }
        """;

        var result = RoundTrip<DataPointResult>(json);

        Assert.Equal("no_cedula", result.FieldKey);
        Assert.Equal("402-1912052-0", result.NormalizedValue);
        Assert.NotEqual(Guid.Empty, result.FileLogId);

        var validation = result.Validation;
        Assert.Equal("passed", validation.Status);
        Assert.Equal(2, validation.Results.Count);
        Assert.Equal(2, validation.Summary.Passed);
        // Exercise the ToString overrides on the validation types.
        _ = validation.ToString();
        _ = validation.Summary.ToString();
        _ = validation.Results[0].ToString();
        _ = validation.Results[1].ToString();
    }

    [Fact]
    public void SessionDetailResult_FullRoundTrip()
    {
        var json = """
        {
          "id": "s1", "creation_date": "2026-01-01T10:00:00Z", "update_date": "2026-01-02T10:00:00Z",
          "status": "open", "user_name": "raul", "company_id": "c1", "job_id": "j1"
        }
        """;

        var result = RoundTrip<SessionDetailResult>(json);

        Assert.Equal("open", result.Status);
        Assert.Equal("j1", result.JobId);
    }

    [Fact]
    public void TagDetailResult_FullRoundTrip()
    {
        var json = """
        {
          "id": "11111111-1111-1111-1111-111111111111", "name": "Invoice", "slug": "invoice",
          "description": "Vendor invoices",
          "company_id": "22222222-2222-2222-2222-222222222222",
          "datapoints": [
            { "id": "99999999-9999-9999-9999-999999999999", "name": "Total", "field_key": "total",
              "display_text": "Total Amount", "data_type": "number", "field_type": "text",
              "value_source": "visual", "required": true, "hidden": false, "read_only": true }
          ],
          "visual_extraction": { "model": "gpt-4" }
        }
        """;

        var result = RoundTrip<TagDetailResult>(json);

        Assert.Equal("Invoice", result.Name);
        Assert.Equal("Vendor invoices", result.Description);
        Assert.Single(result.DataPoints);
        Assert.Equal(Guid.Parse("99999999-9999-9999-9999-999999999999"), result.DataPoints[0].Id);
        Assert.Equal("Total Amount", result.DataPoints[0].DisplayText);
        Assert.True(result.DataPoints[0].ReadOnly);
        _ = result.DataPoints[0].ToString();
        Assert.NotNull(result.VisualExtraction);
    }

    [Fact]
    public void PagedTagResult_RoundTrip()
    {
        var json = """
        {
          "result": [ { "id": "11111111-1111-1111-1111-111111111111", "name": "Invoice", "slug": "invoice", "description": "d" } ],
          "total": 7, "page": 1, "page_size": 50
        }
        """;

        var result = RoundTrip<PagedResult<TagResult>>(json);

        Assert.Equal(7, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
        Assert.Single(result.Result);
        Assert.Equal("Invoice", result.Result[0].Name);
    }

    [Fact]
    public void TagListAndMutationResults_RoundTrip()
    {
        var id = Guid.NewGuid();

        var tag = RoundTrip<TagResult>(
            $$"""{ "id": "{{id}}", "name": "Invoice", "slug": "invoice", "description": "d" }""");
        Assert.Equal("Invoice", tag.Name);

        var created = RoundTrip<CreateTagResult>(
            $$"""{ "id": "{{id}}", "warnings": ["w1", "w2"] }""");
        Assert.Equal(2, created.Warnings.Count);

        var updated = RoundTrip<UpdateTagResult>($$"""{ "id": "{{id}}" }""");
        Assert.Equal(id, updated.Id);
    }

    [Fact]
    public void WhoAmIResult_FullRoundTrip()
    {
        var json = """
        {
          "request_origin_ip_address": "1.2.3.4", "authentication_type": "ApiKey",
          "roles": ["admin", "user"], "user_name": "raul", "name": "Raul", "last_name": "Roa",
          "user_id": "u1", "status": "active", "company_id": "c1", "company_name": "Tagshelf",
          "app_id": "app1", "app_name": "wrapper"
        }
        """;

        var result = RoundTrip<WhoAmIResult>(json);

        Assert.Equal("raul", result.UserName);
        Assert.Equal(2, result.Roles.Count);
        Assert.Equal("Tagshelf", result.CompanyName);
    }
}
