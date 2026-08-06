using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.RealTime;

namespace AlfredApiWrapperTests.RealTime;

public class RealTimeEventTests
{
    [Fact]
    public void FromJson_ParsesCoreDiscriminatorFields()
    {
        var json = """
        {
            "event_name": "alfred_event_job_start",
            "company_id": "company-123",
            "job_id": "job-abc"
        }
        """;

        var evt = RealTimeEvent.FromJson(RealTimeChannels.JobEvent, json);

        Assert.Equal(RealTimeChannels.JobEvent, evt.Channel);
        Assert.Equal("alfred_event_job_start", evt.EventName);
        Assert.Equal("company-123", evt.CompanyId);
        Assert.Equal("job-abc", evt.JobId);
    }

    [Fact]
    public void FromJson_UnwrapsSingleElementArrayPayload()
    {
        // Some transports deliver the payload wrapped in a single-element array.
        var json = """[{ "event_name": "alfred_event_file_done", "company_id": "c1" }]""";

        var evt = RealTimeEvent.FromJson(RealTimeChannels.FileEvent, json);

        Assert.Equal("alfred_event_file_done", evt.EventName);
        Assert.Equal("c1", evt.CompanyId);
    }

    [Fact]
    public void FromJson_MissingFields_YieldNulls_AndPayloadNeverNull()
    {
        var evt = RealTimeEvent.FromJson(RealTimeChannels.JobEvent, "{}");

        Assert.Null(evt.EventName);
        Assert.Null(evt.CompanyId);
        Assert.Null(evt.JobId);
        Assert.NotNull(evt.Payload);
    }

    [Fact]
    public void FromJson_NullOrEmpty_ProducesEmptyPayload()
    {
        var evt = RealTimeEvent.FromJson(RealTimeChannels.JobEvent, null);

        Assert.Null(evt.EventName);
        Assert.NotNull(evt.Payload);
        Assert.Empty(evt.Payload);
    }

    private class FileStatusPayload
    {
        [JsonProperty("event_name")]
        public string? EventName { get; set; }

        [JsonProperty("file_status")]
        public string? FileStatus { get; set; }

        [JsonProperty("classification_score")]
        public double ClassificationScore { get; set; }
    }

    [Fact]
    public void GetData_DeserializesIntoTypedModel_UsingJsonPropertyAttributes()
    {
        var json = """
        {
            "event_name": "alfred_event_file_status_update",
            "file_status": "finished",
            "classification_score": 0.97
        }
        """;

        var evt = RealTimeEvent.FromJson(RealTimeChannels.FileEvent, json);
        var data = evt.GetData<FileStatusPayload>();

        Assert.Equal("alfred_event_file_status_update", data.EventName);
        Assert.Equal("finished", data.FileStatus);
        Assert.Equal(0.97, data.ClassificationScore, precision: 5);
    }

    [Fact]
    public void ToString_IncludesChannelAndEventName()
    {
        var evt = RealTimeEvent.FromJson(RealTimeChannels.JobEvent,
            """{ "event_name": "alfred_event_job_create", "company_id": "c9", "job_id": "j9" }""");

        var text = evt.ToString();

        Assert.Contains(RealTimeChannels.JobEvent, text);
        Assert.Contains("alfred_event_job_create", text);
    }
}
