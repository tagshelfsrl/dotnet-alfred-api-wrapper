using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace AlfredApiWrapperTests.Core;

public class AlfredFactoryTests
{
    [Fact]
    public async Task CreateWithApiKeyAsync_Production_SetsBaseAddressAndHeader()
    {
        var alfred = await AlfredFactory.CreateWithApiKeyAsync("my-key", EnvironmentType.Production);

        Assert.Equal("https://app.tagshelf.com/", alfred.ApiClient.BaseAddress!.ToString());
        Assert.Equal("my-key", alfred.ApiClient.GetDefaultHeaderValue("X-TagshelfAPI-Key"));
        AssertDomainsInitialized(alfred);
    }

    [Fact]
    public async Task CreateWithApiKeyAsync_Staging_SetsStagingBaseAddress()
    {
        var alfred = await AlfredFactory.CreateWithApiKeyAsync("my-key", EnvironmentType.Staging);

        Assert.Equal("https://staging.tagshelf.com/", alfred.ApiClient.BaseAddress!.ToString());
    }

    [Fact]
    public async Task CreateWithHmacAsync_ReturnsInitializedClient()
    {
        var alfred = await AlfredFactory.CreateWithHmacAsync("secret", "key", EnvironmentType.Production);

        Assert.Equal("https://app.tagshelf.com/", alfred.ApiClient.BaseAddress!.ToString());
        AssertDomainsInitialized(alfred);
    }

    [Fact]
    public void Alfred_UnsupportedEnvironment_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Alfred((EnvironmentType)999));
    }

    private static void AssertDomainsInitialized(Alfred alfred)
    {
        Assert.NotNull(alfred.Account);
        Assert.NotNull(alfred.Job);
        Assert.NotNull(alfred.File);
        Assert.NotNull(alfred.Tagshelf);
        Assert.NotNull(alfred.DeferredSession);
        Assert.NotNull(alfred.DataPoint);
        Assert.NotNull(alfred.Tag);
    }
}
