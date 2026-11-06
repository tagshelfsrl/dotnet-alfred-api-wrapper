using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    /// <summary>
    /// One job in a job list. Use <see cref="JobDomain.GetAsync"/> for the job's files and full detail.
    /// </summary>
    public class JobSummaryResult
    {
        /// <summary>The job's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>When the job was created (UTC).</summary>
        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        /// <summary>When the job was last updated (UTC).</summary>
        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; }

        /// <summary>When processing of the job started (UTC), or null when it has not started.</summary>
        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>The user or application that submitted the job.</summary>
        [JsonProperty("user_name")]
        public string UserName { get; set; }

        /// <summary>The channel the job was submitted through (for example "api" or "web").</summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>The correlation ID the job was submitted with; empty when none was given.</summary>
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }

        /// <summary>Whether the job's files were merged into one.</summary>
        [JsonProperty("merge")]
        public bool Merge { get; set; }

        /// <summary>Whether the job's files were split into separate units of work.</summary>
        [JsonProperty("decompose")]
        public bool Decompose { get; set; }

        /// <summary>Whether the job's metadata was applied to each of its files.</summary>
        [JsonProperty("propagate_metadata")]
        public bool PropagateMetadata { get; set; }

        /// <summary>The job's metadata, as JSON text.</summary>
        [JsonProperty("metadata")]
        public string Metadata { get; set; }

        /// <summary>The number of remote input sources of the job.</summary>
        [JsonProperty("file_sources_count")]
        public int FileSourcesCount { get; set; }

        /// <summary>The number of files in the job.</summary>
        [JsonProperty("file_count")]
        public int FileCount { get; set; }

        /// <summary>The number of the job's files that have finished processing.</summary>
        [JsonProperty("finished_files")]
        public int FinishedFiles { get; set; }

        /// <summary>The number of times the job has been retried.</summary>
        [JsonProperty("retries")]
        public int Retries { get; set; }

        /// <summary>Whether the job ran out of retries.</summary>
        [JsonProperty("exceeded_retries")]
        public bool ExceededRetries { get; set; }

        /// <summary>The job's stage (for example "in_progress", "completed" or "failed").</summary>
        [JsonProperty("stage")]
        public string Stage { get; set; }

        /// <summary>The job's priority (for example "normal" or "high").</summary>
        [JsonProperty("priority")]
        public string Priority { get; set; }

        /// <summary>Whether any of the job's files is waiting for human review.</summary>
        [JsonProperty("has_confirmation_required_files")]
        public bool HasConfirmationRequiredFiles { get; set; }

        /// <summary>The number of the job's files waiting for human review.</summary>
        [JsonProperty("confirmation_required_files_count")]
        public int ConfirmationRequiredFilesCount { get; set; }

        /// <summary>Whether any of the job's files failed.</summary>
        [JsonProperty("has_failed_files")]
        public bool HasFailedFiles { get; set; }

        /// <summary>The number of the job's files that failed.</summary>
        [JsonProperty("failed_files_count")]
        public int FailedFilesCount { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that contains the Id, Stage and CreationDate.</returns>
        public override string ToString()
        {
            return $"{Id} - {Stage} - {CreationDate}";
        }
    }
}
