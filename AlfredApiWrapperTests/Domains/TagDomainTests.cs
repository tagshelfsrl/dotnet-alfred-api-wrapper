using System.Net.Http;
using AlfredApiWrapperTests.TestHelpers;
using TagShelf.Alfred.ApiWrapper.Domains.Tag;
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Requests;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace AlfredApiWrapperTests.Domains;

public class TagDomainTests
{
    [Fact]
    public void Constructor_RejectsNullClient()
    {
        Assert.Throws<ArgumentNullException>(() => new TagDomain(null!));
    }

    // ---- Listing (unpaged) ----

    [Fact]
    public async Task GetAllAsync_HitsTagsEndpoint_AndMapsBareArray()
    {
        var handler = FakeHttpMessageHandler.Json("""
        [
          { "id": "11111111-1111-1111-1111-111111111111", "name": "Invoice", "slug": "invoice", "description": "Invoices" },
          { "id": "22222222-2222-2222-2222-222222222222", "name": "Receipt", "slug": "receipt", "description": null }
        ]
        """);
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Invoice", result[0].Name);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        // No filters set -> no query string.
        Assert.Equal("https://example.test/api/tags", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetAllAsync_AppliesQueryAndIncludeDisabled()
    {
        var handler = FakeHttpMessageHandler.Json("[]");
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        await domain.GetAllAsync(query: "inv oice", includeDisabled: true);

        // AbsoluteUri preserves percent-encoding (ToString() would unescape it).
        var uri = handler.LastRequest!.RequestUri!.AbsoluteUri;
        Assert.Contains("query=inv%20oice", uri);
        Assert.Contains("include_disabled=true", uri);
    }

    // ---- Listing (paged) ----

    [Fact]
    public async Task GetPagedAsync_ReturnsEnvelope_AndSendsPageParams()
    {
        var handler = FakeHttpMessageHandler.Json("""
        {
          "result": [ { "id": "11111111-1111-1111-1111-111111111111", "name": "Invoice", "slug": "invoice", "description": "d" } ],
          "total": 42, "page": 2, "page_size": 25
        }
        """);
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetPagedAsync(page: 2, pageSize: 25);

        Assert.Equal(42, result.Total);
        Assert.Equal(2, result.Page);
        Assert.Equal(25, result.PageSize);
        Assert.Single(result.Result);
        Assert.Equal("Invoice", result.Result[0].Name);
        var uri = handler.LastRequest!.RequestUri!.ToString();
        Assert.Contains("page=2", uri);
        Assert.Contains("page_size=25", uri);
    }

    // ---- Create ----

    [Fact]
    public async Task CreateAsync_PostsNameAndDescription_AndReturnsIdWithWarnings()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json(
            $$"""{ "id": "{{id}}", "warnings": ["Tag created without a description."] }""");
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.CreateAsync(new CreateTagRequest { Name = "Invoice", Description = "Invoices" });

        Assert.Equal(id, result.Id);
        Assert.Single(result.Warnings);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.EndsWith("/api/tag", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("\"name\":\"Invoice\"", handler.LastRequestBody);
        Assert.Contains("\"description\":\"Invoices\"", handler.LastRequestBody);
    }

    // ---- Detail (slim + expand) ----

    [Fact]
    public async Task GetAsync_Default_IsSlim_NoExpandQuery()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""
        {
          "id": "{{id}}", "name": "Invoice", "slug": "invoice", "description": "Vendor invoices",
          "company_id": "33333333-3333-3333-3333-333333333333",
          "datapoints": [
            { "id": "44444444-4444-4444-4444-444444444444", "name": "Total", "field_key": "total" }
          ]
        }
        """);
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetAsync(id);

        Assert.Equal("Invoice", result.Name);
        Assert.Equal("Vendor invoices", result.Description);
        Assert.Equal(Guid.Parse("33333333-3333-3333-3333-333333333333"), result.CompanyId);
        Assert.Single(result.DataPoints);
        Assert.Equal(Guid.Parse("44444444-4444-4444-4444-444444444444"), result.DataPoints[0].Id);
        Assert.Equal("total", result.DataPoints[0].FieldKey);
        // Slim datapoint: configuration fields not present.
        Assert.Null(result.DataPoints[0].DataType);
        Assert.Null(result.VisualExtraction);
        // No expand requested -> bare detail URL.
        Assert.EndsWith($"/api/tag/{id}", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetAsync_ExpandDataPoints_SendsExpandQuery_AndMapsFullConfig()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""
        {
          "id": "{{id}}", "name": "Invoice", "slug": "invoice", "description": "d",
          "company_id": "33333333-3333-3333-3333-333333333333",
          "datapoints": [
            { "id": "44444444-4444-4444-4444-444444444444", "name": "Total", "field_key": "total",
              "display_text": "Total Amount", "data_type": "number", "field_type": "text",
              "value_source": "visual", "required": true, "hidden": false, "read_only": true }
          ]
        }
        """);
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetAsync(id, TagExpand.DataPoints);

        Assert.Equal("Total Amount", result.DataPoints[0].DisplayText);
        Assert.Equal("visual", result.DataPoints[0].ValueSource);
        Assert.True(result.DataPoints[0].ReadOnly);
        Assert.Contains("expand=datapoints", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetAsync_ExpandAll_RequestsBothExpansions()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""
        {
          "id": "{{id}}", "name": "Invoice", "datapoints": [],
          "visual_extraction": { "model": "gpt-4" }
        }
        """);
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.GetAsync(id, TagExpand.All);

        Assert.NotNull(result.VisualExtraction);
        Assert.Equal("gpt-4", (string)result.VisualExtraction["model"]!);
        var uri = Uri.UnescapeDataString(handler.LastRequest!.RequestUri!.ToString());
        Assert.Contains("expand=datapoints,visual_extraction", uri);
    }

    [Theory]
    [InlineData(TagExpand.None, null)]
    [InlineData(TagExpand.DataPoints, "datapoints")]
    [InlineData(TagExpand.VisualExtraction, "visual_extraction")]
    [InlineData(TagExpand.All, "datapoints,visual_extraction")]
    public void BuildExpandValue_MapsFlags(TagExpand expand, string? expected)
    {
        Assert.Equal(expected, TagDomain.BuildExpandValue(expand));
    }

    [Fact]
    public void BuildQueryString_SkipsEmptyAndEncodes()
    {
        var qs = TagDomain.BuildQueryString(
            new KeyValuePair<string, string>("a", "1"),
            new KeyValuePair<string, string>("b", null),
            new KeyValuePair<string, string>("c", "x y"));

        Assert.Equal("?a=1&c=x%20y", qs);
        Assert.Equal(string.Empty, TagDomain.BuildQueryString(new KeyValuePair<string, string>("a", "")));
    }

    // ---- Update ----

    [Fact]
    public async Task UpdateAsync_SendsPatch_AndOmitsUnsetFields()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{id}}" }""");
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        var result = await domain.UpdateAsync(id, new UpdateTagRequest { Description = "Updated" });

        Assert.Equal(id, result.Id);
        Assert.Equal("PATCH", handler.LastRequest!.Method.Method);
        Assert.EndsWith($"/api/tag/{id}", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("\"description\":\"Updated\"", handler.LastRequestBody);
        Assert.DoesNotContain("disabled", handler.LastRequestBody);
    }

    [Fact]
    public async Task UpdateAsync_CanToggleDisabled()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{id}}" }""");
        var domain = new TagDomain(FakeHttpMessageHandler.Client(handler));

        await domain.UpdateAsync(id, new UpdateTagRequest { Disabled = true });

        Assert.Contains("\"disabled\":true", handler.LastRequestBody);
        Assert.DoesNotContain("description", handler.LastRequestBody);
    }
}
