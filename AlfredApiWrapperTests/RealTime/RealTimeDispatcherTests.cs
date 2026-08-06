using TagShelf.Alfred.ApiWrapper.RealTime;

namespace AlfredApiWrapperTests.RealTime;

public class RealTimeDispatcherTests
{
    private static RealTimeEvent Event(string channel, string eventName) =>
        RealTimeEvent.FromJson(channel, $$"""{ "event_name": "{{eventName}}" }""");

    [Fact]
    public void ChannelHandler_ReceivesEveryEventOnChannel()
    {
        var dispatcher = new RealTimeDispatcher();
        var received = new List<string>();
        dispatcher.AddChannelHandler(RealTimeChannels.JobEvent, e => received.Add(e.EventName));

        foreach (var h in dispatcher.ResolveHandlers(RealTimeChannels.JobEvent, "alfred_event_job_start"))
            h(Event(RealTimeChannels.JobEvent, "alfred_event_job_start"));
        foreach (var h in dispatcher.ResolveHandlers(RealTimeChannels.JobEvent, "alfred_event_job_create"))
            h(Event(RealTimeChannels.JobEvent, "alfred_event_job_create"));

        Assert.Equal(new[] { "alfred_event_job_start", "alfred_event_job_create" }, received);
    }

    [Fact]
    public void EventNameHandler_OnlyFiresForMatchingEventName()
    {
        var dispatcher = new RealTimeDispatcher();
        var starts = 0;
        dispatcher.AddEventNameHandler(RealTimeChannels.JobEvent, JobEventNames.JobStart, _ => starts++);

        // Matching event name -> handler resolved.
        Assert.Single(dispatcher.ResolveHandlers(RealTimeChannels.JobEvent, JobEventNames.JobStart));
        // Different event name -> not resolved.
        Assert.Empty(dispatcher.ResolveHandlers(RealTimeChannels.JobEvent, JobEventNames.JobCreate));
        // Same event name, different channel -> not resolved.
        Assert.Empty(dispatcher.ResolveHandlers(RealTimeChannels.FileEvent, JobEventNames.JobStart));
    }

    [Fact]
    public void ChannelAndEventNameHandlers_BothResolveForMatchingEvent()
    {
        var dispatcher = new RealTimeDispatcher();
        dispatcher.AddChannelHandler(RealTimeChannels.FileEvent, _ => { });
        dispatcher.AddEventNameHandler(RealTimeChannels.FileEvent, FileEventNames.FileDone, _ => { });

        var handlers = dispatcher.ResolveHandlers(RealTimeChannels.FileEvent, FileEventNames.FileDone);

        Assert.Equal(2, handlers.Count);
    }

    [Fact]
    public void MultipleHandlersOnSameChannel_AllResolved()
    {
        var dispatcher = new RealTimeDispatcher();
        dispatcher.AddChannelHandler(RealTimeChannels.JobEvent, _ => { });
        dispatcher.AddChannelHandler(RealTimeChannels.JobEvent, _ => { });

        Assert.Equal(2, dispatcher.ResolveHandlers(RealTimeChannels.JobEvent, "anything").Count);
    }

    [Fact]
    public void NullEventName_ResolvesChannelHandlersOnly()
    {
        var dispatcher = new RealTimeDispatcher();
        dispatcher.AddChannelHandler(RealTimeChannels.JobEvent, _ => { });
        dispatcher.AddEventNameHandler(RealTimeChannels.JobEvent, JobEventNames.JobStart, _ => { });

        var handlers = dispatcher.ResolveHandlers(RealTimeChannels.JobEvent, null);

        Assert.Single(handlers);
    }
}
