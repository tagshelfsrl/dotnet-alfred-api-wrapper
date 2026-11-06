using System.Net;
using System.Net.Http;
using AlfredApiWrapperTests.TestHelpers;
using Newtonsoft.Json.Linq;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DataPoint;

namespace AlfredApiWrapperTests.Domains;

public class DataPointDomainTests
{
    private static readonly Guid TagId = Guid.Parse("0bb267ab-2a58-4123-9550-1d6043b161db");

    [Fact]
    public async Task GetValuesAsync_MapsExtractedValue()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        [ { "id": "{{Guid.NewGuid()}}", "metadata_name": "InvoiceTotal", "value": "260000.00", "extracted_value": "260,000.00" } ]
        """);
        var domain = new DataPointDomain(FakeHttpMessageHandler.Client(handler));

        var values = await domain.GetValuesAsync(Guid.NewGuid());

        Assert.Equal("260,000.00", Assert.Single(values).ExtractedValue);
    }

    [Fact]
    public async Task GetValuesByTagAsync_ReturnsFlatRowsWithDynamicKeys()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        [ { "file_id": "{{Guid.NewGuid()}}", "tag_name": "Invoice", "file_size": 304525, "invoice_number": "RMP002-26", "parent_id": null } ]
        """);
        var domain = new DataPointDomain(FakeHttpMessageHandler.Client(handler));

        var rows = await domain.GetValuesByTagAsync(TagId);

        Assert.Equal($"https://example.test/api/values/tag/{TagId}", handler.LastRequest!.RequestUri!.ToString());
        var row = Assert.Single(rows);
        Assert.Equal("RMP002-26", row["invoice_number"].Value<string>());
        Assert.Equal(304525, row["file_size"].Value<long>());
        Assert.Equal(JTokenType.Null, row["parent_id"].Type);
    }

    [Fact]
    public async Task GetDefinitionsAsync_MapsDefinitionsWithRules()
    {
        var handler = FakeHttpMessageHandler.Json($$"""
        [ {
            "id": "{{Guid.NewGuid()}}", "tag_id": "{{TagId}}", "name": "PSSNombre", "tag_name": "Reclamaciones Medicas",
            "field_key": "pss_nombre", "field_type": "String", "is_pii": true, "pii_mask": "***",
            "formatters": [ { "type": "trim" } ], "validators": [ { "type": "required" } ]
        } ]
        """);
        var domain = new DataPointDomain(FakeHttpMessageHandler.Client(handler));

        var definitions = await domain.GetDefinitionsAsync(TagId);

        Assert.Equal($"https://example.test/api/metadata/tag/{TagId}", handler.LastRequest!.RequestUri!.ToString());
        var d = Assert.Single(definitions);
        Assert.Equal("pss_nombre", d.FieldKey);
        Assert.True(d.IsPii);
        Assert.Equal("***", d.PiiMask);
        Assert.Equal("required", d.Validators![0]!["type"]!.Value<string>());
        Assert.Null(d.StructuredSchema);
    }

    [Fact]
    public async Task GetDefinitionAsync_HitsDetailRoute()
    {
        var id = Guid.NewGuid();
        var handler = FakeHttpMessageHandler.Json($$"""{ "id": "{{id}}", "tag_id": "{{TagId}}", "name": "Total", "tag_name": "", "field_type": "Decimal" }""");
        var domain = new DataPointDomain(FakeHttpMessageHandler.Client(handler));

        var d = await domain.GetDefinitionAsync(id);

        Assert.Equal($"https://example.test/api/metadata/detail/{id}", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Decimal", d.FieldType);
    }
}
