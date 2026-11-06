using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    /// <summary>
    /// An event Alfred raised for a file (for example when it finished processing), and whether it was delivered.
    /// </summary>
    public class FileEventResult
    {
        /// <summary>The event's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>When the event was raised (UTC).</summary>
        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        /// <summary>When the event record was last updated (UTC).</summary>
        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; }

        /// <summary>The file the event belongs to.</summary>
        [JsonProperty("file_id")]
        public Guid FileId { get; set; }

        /// <summary>The event's name (for example "alfred_event_file_done").</summary>
        [JsonProperty("event_name")]
        public string EventName { get; set; }

        /// <summary>The event's payload, as JSON text.</summary>
        [JsonProperty("event_envelope")]
        public string EventEnvelope { get; set; }

        /// <summary>Whether the event was published to the event broker (real-time events).</summary>
        [JsonProperty("sent_to_event_broker")]
        public bool SentToEventBroker { get; set; }

        /// <summary>Whether the event was delivered to the account's webhook.</summary>
        [JsonProperty("sent_to_web_hook")]
        public bool SentToWebHook { get; set; }
    }
}
