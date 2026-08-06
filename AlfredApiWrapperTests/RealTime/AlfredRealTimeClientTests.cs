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

    [Fact]
    public void Dispatch_InvokesChannelAndEventNameHandlers()
    {
        using var client = AlfredFactory.CreateRealTimeClient("key");
        var channelHits = 0;
        string? filteredEventName = null;
        client.OnJobEvent(_ => channelHits++);
        client.On(RealTimeChannels.JobEvent, JobEventNames.JobStart, e => filteredEventName = e.EventName);

        client.Dispatch(RealTimeChannels.JobEvent,
            $$"""{ "event_name": "{{JobEventNames.JobStart}}", "job_id": "j1" }""");

        Assert.Equal(1, channelHits);
        Assert.Equal(JobEventNames.JobStart, filteredEventName);
    }

    [Fact]
    public void Dispatch_EventNameHandler_DoesNotFireForOtherEvents()
    {
        using var client = AlfredFactory.CreateRealTimeClient("key");
        var startHits = 0;
        client.On(RealTimeChannels.JobEvent, JobEventNames.JobStart, _ => startHits++);

        client.Dispatch(RealTimeChannels.JobEvent,
            $$"""{ "event_name": "{{JobEventNames.JobCreate}}" }""");

        Assert.Equal(0, startHits);
    }

    [Fact]
    public void Dispatch_MalformedJson_RaisesErrorEvent_WithoutThrowing()
    {
        using var client = AlfredFactory.CreateRealTimeClient("key",
            EnvironmentType.Production, new RealTimeOptions { Verbose = true });
        string? error = null;
        client.Error += (_, e) => error = e;

        client.Dispatch(RealTimeChannels.JobEvent, "{ this is not json");

        Assert.NotNull(error);
        Assert.Contains("Failed to parse", error);
    }

    [Fact]
    public void Dispatch_HandlerThatThrows_RaisesErrorEvent_AndOtherHandlersStillRun()
    {
        using var client = AlfredFactory.CreateRealTimeClient("key",
            EnvironmentType.Production, new RealTimeOptions { Verbose = true });
        string? error = null;
        var secondHandlerRan = false;
        client.Error += (_, e) => error = e;
        client.OnFileEvent(_ => throw new InvalidOperationException("boom"));
        client.OnFileEvent(_ => secondHandlerRan = true);

        client.Dispatch(RealTimeChannels.FileEvent,
            $$"""{ "event_name": "{{FileEventNames.FileDone}}" }""");

        Assert.NotNull(error);
        Assert.Contains("Handler error", error);
        Assert.True(secondHandlerRan);
    }

    [Fact]
    public async Task DisconnectAsync_WhenNotConnected_CompletesQuietly()
    {
        using var client = AlfredFactory.CreateRealTimeClient("key");

        await client.DisconnectAsync(); // no connection established; must not throw
        Assert.False(client.IsConnected);
    }
}
