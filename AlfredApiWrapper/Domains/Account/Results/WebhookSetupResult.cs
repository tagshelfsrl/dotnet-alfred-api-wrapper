using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Account.Results
{
    public class WebhookSetupResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that contains the webhook Id.</returns>
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}