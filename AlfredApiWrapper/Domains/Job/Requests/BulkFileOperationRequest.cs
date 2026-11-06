using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Requests
{
    /// <summary>
    /// Reprocesses or re-extracts a chosen set of a job's files in one call.
    /// </summary>
    public class BulkFileOperationRequest
    {
        /// <summary>The files of the job to act on. Files that are not in the job, or not eligible, are skipped and reported.</summary>
        [JsonProperty("file_ids")]
        public List<Guid> FileIds { get; set; } = new List<Guid>();

        /// <summary>The operation to run on each file.</summary>
        [JsonIgnore]
        public FileOperation Operation { get; set; } = FileOperation.Reprocess;

        /// <summary>The wire value of <see cref="Operation"/>.</summary>
        [JsonProperty("op")]
        internal string Op => Operation == FileOperation.Reextract ? "reextract" : "reprocess";

        /// <summary>
        /// Hold every affected file for human review, whatever its tag's validation settings say.
        /// Defaults to false.
        /// </summary>
        [JsonProperty("request_review")]
        public bool RequestReview { get; set; }
    }
}
