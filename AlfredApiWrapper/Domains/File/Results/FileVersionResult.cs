using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    /// <summary>
    /// One version of a file's extracted text.
    /// </summary>
    public class FileVersionResult
    {
        /// <summary>The version's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>When the version was created (UTC).</summary>
        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        /// <summary>When the version was last updated (UTC).</summary>
        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; }

        /// <summary>The processing record (file log) the version belongs to.</summary>
        [JsonProperty("file_log_id")]
        public Guid FileLogId { get; set; }

        /// <summary>The text extracted from the file in this version.</summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>The rotation, in degrees, applied to the file for this version.</summary>
        [JsonProperty("rotate")]
        public int Rotate { get; set; }

        /// <summary>The page orientation detected for this version.</summary>
        [JsonProperty("orientation")]
        public int Orientation { get; set; }
    }
}
