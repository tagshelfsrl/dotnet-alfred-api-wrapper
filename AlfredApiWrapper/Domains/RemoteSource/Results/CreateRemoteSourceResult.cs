using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.RemoteSource.Results
{
    /// <summary>
    /// The identifier of a newly created remote source.
    /// </summary>
    public class CreateRemoteSourceResult
    {
        /// <summary>The new source's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}
