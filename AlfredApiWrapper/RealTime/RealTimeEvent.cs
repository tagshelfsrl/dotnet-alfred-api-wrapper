using Newtonsoft.Json.Linq;

namespace TagShelf.Alfred.ApiWrapper.RealTime
{
    /// <summary>
    /// Represents a single real-time event received from the Alfred socket server.
    /// The server emits on two aggregate channels (<c>job_event</c> / <c>file_event</c>); the specific
    /// event type is carried in the inner <see cref="EventName"/> field. The full, raw payload is
    /// available via <see cref="Payload"/> and can be deserialized into a typed model with <see cref="GetData{T}"/>.
    /// </summary>
    public class RealTimeEvent
    {
        /// <summary>The Socket.IO channel this event arrived on (e.g. <c>job_event</c> or <c>file_event</c>).</summary>
        public string Channel { get; }

        /// <summary>
        /// The inner <c>event_name</c> discriminator (e.g. <c>alfred_event_job_start</c>), or <c>null</c>
        /// if the payload did not contain one. See <see cref="JobEventNames"/> / <see cref="FileEventNames"/>.
        /// </summary>
        public string EventName { get; }

        /// <summary>The tenant/company identifier the event belongs to, when present.</summary>
        public string CompanyId { get; }

        /// <summary>The related job identifier, when present.</summary>
        public string JobId { get; }

        /// <summary>The full, raw event payload as a <see cref="JObject"/>. Never <c>null</c>.</summary>
        public JObject Payload { get; }

        internal RealTimeEvent(string channel, JObject payload)
        {
            Channel = channel;
            Payload = payload ?? new JObject();
            EventName = (string)Payload["event_name"];
            CompanyId = (string)Payload["company_id"];
            JobId = (string)Payload["job_id"];
        }

        /// <summary>
        /// Deserializes the raw payload into a strongly typed model using Newtonsoft.Json.
        /// Decorate your model with <c>[JsonProperty("snake_case")]</c> attributes to map fields.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The deserialized model.</returns>
        public T GetData<T>()
        {
            return Payload.ToObject<T>();
        }

        /// <summary>
        /// Builds a <see cref="RealTimeEvent"/> from the raw JSON string extracted from the socket response.
        /// The socket forwards the event payload as a JSON object; some transports wrap it in a single-element
        /// array, which is normalized here.
        /// </summary>
        internal static RealTimeEvent FromJson(string channel, string json)
        {
            JObject payload = null;
            if (!string.IsNullOrWhiteSpace(json))
            {
                var token = JToken.Parse(json);
                payload = NormalizeToObject(token);
            }
            return new RealTimeEvent(channel, payload);
        }

        private static JObject NormalizeToObject(JToken token)
        {
            if (token is JObject obj)
            {
                return obj;
            }
            if (token is JArray array)
            {
                foreach (var element in array)
                {
                    if (element is JObject elementObject)
                    {
                        return elementObject;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a concise description of the event, including the channel, event name, company and job.
        /// </summary>
        public override string ToString()
        {
            return $"{Channel}/{EventName ?? "(none)"} (company={CompanyId}, job={JobId})";
        }
    }
}
