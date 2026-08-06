using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Enumerations;
using TagShelf.Alfred.ApiWrapper.RealTime;

namespace AlfredApiWrapperTests.RealTime;

public class AlfredRealTimeClientTests
{
    [Fact]
    public void ResolveUrl_DefaultsToSocketsEndpoint_ForBothEnvironments()
    {
        Assert.Equal("https://sockets.tagshelf.io",
            AlfredRealTimeClient.ResolveUrl(EnvironmentType.Production, null));
        Assert.Equal("https://sockets.tagshelf.io",
            AlfredRealTimeClient.ResolveUrl(EnvironmentType.Staging, null));
    }

    [Fact]
    public void ResolveUrl_HonorsExplicitOverride()
    {
        var options = new RealTimeOptions { RealTimeUrl = "https://localhost:9999" };

        Assert.Equal("https://localhost:9999",
            AlfredRealTimeClient.ResolveUrl(EnvironmentType.Production, options));
    }

    [Fact]
    public void ResolveUrl_IgnoresWhitespaceOverride()
    {
        var options = new RealTimeOptions { RealTimeUrl = "   " };

        Assert.Equal("https://sockets.tagshelf.io",
            AlfredRealTimeClient.ResolveUrl(EnvironmentType.Production, options));
    }

    [Fact]
    public void Factory_RequiresApiKey()
    {
        Assert.Throws<ArgumentException>(() => AlfredFactory.CreateRealTimeClient(""));
        Assert.Throws<ArgumentException>(() => AlfredFactory.CreateRealTimeClient("   "));
    }

    [Fact]
    public void Factory_CreatesUnconnectedClient_WithResolvedUrl()
    {
        using var client = AlfredFactory.CreateRealTimeClient("some-api-key", EnvironmentType.Production);

        Assert.False(client.IsConnected);
        Assert.Equal("https://sockets.tagshelf.io", client.Url);
    }

    [Fact]
    public void On_RejectsInvalidArguments()
    {
        using var client = AlfredFactory.CreateRealTimeClient("some-api-key");

        Assert.Throws<ArgumentException>(() => client.On("", _ => { }));
        Assert.Throws<ArgumentNullException>(() => client.On(RealTimeChannels.JobEvent, null));
        Assert.Throws<ArgumentException>(() => client.On(RealTimeChannels.JobEvent, "", _ => { }));
    }

    [Fact]
    public void RealTimeOptions_HaveExpectedDefaults()
    {
        var options = new RealTimeOptions();

        Assert.True(options.Reconnection);
        Assert.Equal(int.MaxValue, options.ReconnectionAttempts);
        Assert.Equal(1000, options.ReconnectionDelay);
        Assert.Equal(20000, options.ConnectionTimeout);
        Assert.False(options.Verbose);
        Assert.Null(options.RealTimeUrl);
    }
}
