using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    /// <summary>
    /// Confirms that a file was queued for reprocessing or re-extraction.
    /// <para>
    /// A file that cannot be queued is raised as an <see cref="Core.ApiException"/>: 409 when a run already
    /// in progress holds the file, 400 otherwise (for example re-extracting a file that has no tag). The
    /// exception's <c>ErrorMessage</c> says why.
    /// </para>
    /// </summary>
    public class FileOperationResult
    {
        /// <summary>The file's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>Whether the file will be held for human review when the run finishes.</summary>
        [JsonProperty("request_review")]
        public bool RequestReview { get; set; }

        /// <summary>For a reprocess, whether cached intermediate results were bypassed. Always false for a re-extract.</summary>
        [JsonProperty("skip_cache")]
        public bool SkipCache { get; set; }
    }
}
