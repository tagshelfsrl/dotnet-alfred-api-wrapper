using System.Net;
using System.Net.Http;
using AlfredApiWrapperTests.TestHelpers;
using Newtonsoft.Json.Linq;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains;

namespace AlfredApiWrapperTests.Domains;

public class AccountDomainTests
{
    [Fact]
    public async Task GetCompaniesAsync_MapsCompanies()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""[ { "id": "{{id}}", "name": "Wayne Enterprises" } ]""");
        var domain = new AccountDomain(FakeHttpMessageHandler.Client(handler));

        var companies = await domain.GetCompaniesAsync();

        Assert.Equal("https://example.test/api/account/companies", handler.LastRequest!.RequestUri!.ToString());
        var company = Assert.Single(companies);
        Assert.Equal(id, company.Id);
        Assert.Equal("Wayne Enterprises", company.Name);
    }

    [Fact]
    public async Task GetTagsAsync_WithoutCompany_UsesOwnAccount()
    {
        var handler = FakeHttpMessageHandler.Json($$"""[ { "id": "{{Guid.NewGuid()}}", "name": "Transaction Slip", "description": "d" } ]""");
        var domain = new AccountDomain(FakeHttpMessageHandler.Client(handler));

        var tags = await domain.GetTagsAsync();

        Assert.Equal("https://example.test/api/account/tags", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Transaction Slip", Assert.Single(tags).Name);
    }

    [Fact]
    public async Task GetTagsAsync_WithCompany_PutsItInThePath()
    {
        var companyId = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json("[]");
        var domain = new AccountDomain(FakeHttpMessageHandler.Client(handler));

        await domain.GetTagsAsync(companyId);

        Assert.Equal($"https://example.test/api/account/tags/{companyId}", handler.LastRequest!.RequestUri!.ToString());
    }
}
