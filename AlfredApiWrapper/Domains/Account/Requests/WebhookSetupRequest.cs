using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Account.Requests
{
    public class WebhookSetupRequest
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}