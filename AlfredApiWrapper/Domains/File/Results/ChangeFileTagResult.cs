using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    /// <summary>
    /// Confirms that a file's tag (document type) was changed and the file was sent to extraction under it.
    /// </summary>
    public class ChangeFileTagResult
    {
        /// <summary>The file's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>The file's new tag.</summary>
        [JsonProperty("tag_id")]
        public Guid TagId { get; set; }
    }
}
