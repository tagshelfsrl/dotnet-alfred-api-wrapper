using System.Net;
using System.Net.Http;
using AlfredApiWrapperTests.TestHelpers;
using Newtonsoft.Json.Linq;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Search;
using TagShelf.Alfred.ApiWrapper.Domains.Search.Requests;

namespace AlfredApiWrapperTests.Domains;

public class SearchDomainTests
{
    private const string Page = """
    {
      "result": [ {
        "file_id": "d2bd5c6f-cd2a-490b-b93b-ee843ceba9ea", "job_id": "44f9423e-f36b-1410-8c10-0092b3381c4b",
        "channel": "web", "user_name": "admin@tagshelf.com", "length": 576.0, "tag_name": "Factura Fiscal",
        "tag_score": 0.95, "tag_source": "Alfred", "classifier": "zero", "tag_id": "61b5433e-f36b-1410-8c0f-0092b3381c4b",
        "creation_date": "2025-06-04T09:17:17.8Z", "update_date": "2025-06-04T09:23:16.703Z",
        "data_points": [
          { "name": "InvoiceNumber", "value": "RMP002-26", "value_as_string": "RMP002-26", "data_type": "String" },
          { "name": "InvoiceTotal", "value": 260000.0, "value_as_string": "260000.00", "data_type": "Decimal" }
        ]
      } ],
      "total": 1, "is_empty": false
    }
    """;

    [Fact]
    public async Task BasicAsync_SendsFilters_AndMapsTypedValues()
    {
        var handler = FakeHttpMessageHandler.Json(Page);
        var domain = new SearchDomain(FakeHttpMessageHandler.Client(handler));
        var tagId = Guid.NewGuid();

        var page = await domain.BasicAsync(new SearchRequest { Query = "RMP 002", TagId = tagId, PageSize = 10, Fuzzy = true });

        var uri = handler.LastRequest!.RequestUri!.AbsoluteUri;
        Assert.StartsWith("https://example.test/api/search/basic?", uri);
        Assert.Contains("query=RMP%20002", uri);
        Assert.Contains($"tagId={tagId}", uri);
        Assert.Contains("page=1", uri);
        Assert.Contains("pageSize=10", uri);
        Assert.Contains("fuzzy=true", uri);
        Assert.DoesNotContain("tagName", uri);

        var hit = Assert.Single(page.Result);
        Assert.Equal("Factura Fiscal", hit.TagName);
        Assert.Equal(260000.0m, hit.DataPoints[1].Value.Value<decimal>());
        Assert.Equal("260000.00", hit.DataPoints[1].ValueAsString);
    }

    [Fact]
    public async Task UniversalAsync_HitsUniversalRoute()
    {
        var handler = FakeHttpMessageHandler.Json(Page);
        var domain = new SearchDomain(FakeHttpMessageHandler.Client(handler));

        var page = await domain.UniversalAsync("factura", page: 2);

        Assert.Equal("https://example.test/api/search/universal?query=factura&page=2&pageSize=50", handler.LastRequest!.RequestUri!.AbsoluteUri);
        Assert.Equal(1, page.Total);
    }

    [Fact]
    public async Task Search_RequiresAQuery()
    {
        var domain = new SearchDomain(FakeHttpMessageHandler.Client("{}"));

        await Assert.ThrowsAsync<ArgumentException>(() => domain.BasicAsync(new SearchRequest()));
        await Assert.ThrowsAsync<ArgumentException>(() => domain.UniversalAsync(" "));
        Assert.Throws<ArgumentNullException>(() => new SearchDomain(null!));
    }
}
