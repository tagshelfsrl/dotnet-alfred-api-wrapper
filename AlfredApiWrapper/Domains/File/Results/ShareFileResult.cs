using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    /// <summary>
    /// A shareable link to a file.
    /// </summary>
    public class ShareFileResult
    {
        /// <summary>The shared file's unique identifier.</summary>
        [JsonProperty("file_id")]
        public Guid FileId { get; set; }

        /// <summary>
        /// The link. Anyone who has it can download the file without credentials until it expires; after that
        /// it answers 410 Gone.
        /// </summary>
        [JsonProperty("share_url")]
        public string ShareUrl { get; set; }
    }
}
