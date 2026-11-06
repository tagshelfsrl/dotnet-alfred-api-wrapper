using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    /// <summary>
    /// The outcome of a bulk reprocess or re-extract: one entry per requested file.
    /// </summary>
    public class BulkFileOperationResult
    {
        /// <summary>One entry per requested file.</summary>
        [JsonProperty("results")]
        public List<BulkFileOperationOutcome> Results { get; set; }
    }

    /// <summary>
    /// What happened to one file of a bulk operation.
    /// </summary>
    public class BulkFileOperationOutcome
    {
        /// <summary>The file's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>What happened to the file: "done", "skipped" or "failed".</summary>
        [JsonProperty("outcome")]
        public string Outcome { get; set; }

        /// <summary>Why the file was skipped or failed; empty when it was done.</summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
