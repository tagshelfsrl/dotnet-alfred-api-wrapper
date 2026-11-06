using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.RemoteSource.Results
{
    /// <summary>
    /// A configured remote source. Its credentials are never returned.
    /// </summary>
    public class RemoteSourceResult
    {
        /// <summary>The source's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>The name uploads use to refer to the source.</summary>
        [JsonProperty("source_name")]
        public string SourceName { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The source name.</returns>
        public override string ToString()
        {
            return SourceName;
        }
    }
}
