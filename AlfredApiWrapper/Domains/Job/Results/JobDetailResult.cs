using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    public class JobDetailResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        [Obsolete("The API no longer returns this field; it is always the default value.")]
        [JsonProperty("has_job_request_info")]
        public bool HasJobRequestInfo { get; set; }

        [Obsolete("The API no longer returns this field; it is always the default value.")]
        [JsonProperty("job_request_date")]
        public DateTime? JobRequestDate { get; set; }

        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty("bulk_id")]
        public string BulkId { get; set; }

        [JsonProperty("deferred_session_id")]
        public string DeferredSessionId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("container")]
        public string Container { get; set; }

        [JsonProperty("remote_file_name")]
        public string RemoteFileName { get; set; }

        [Obsolete("The API no longer returns this field; it is always the default value.")]
        [JsonProperty("remote_file_names")]
        public List<string> RemoteFileNames { get; set; }

        [JsonProperty("merge")]
        public bool Merge { get; set; }

        [JsonProperty("decompose")]
        public bool Decompose { get; set; }

        [JsonProperty("propagate_metadata")]
        public bool PropagateMetadata { get; set; }

        [JsonProperty("parent_file_prefix")]
        public string ParentFilePrefix { get; set; }

        [Obsolete("The API no longer returns this field; it is always the default value.")]
        [JsonProperty("decomposed_page_rotation")]
        public int DecomposedPageRotation { get; set; }

        [JsonProperty("file_count")]
        public int FileCount { get; set; }

        [JsonProperty("file_sources_count")]
        public int FileSourcesCount { get; set; }

        [JsonProperty("metadata_objects_count")]
        public int MetadataObjectsCount { get; set; }

        [JsonProperty("finished_files")]
        public int FinishedFiles { get; set; }

        [JsonProperty("files")]
        public List<JobFileDetail> Files { get; set; }

        [JsonProperty("retries")]
        public int Retries { get; set; }

        [JsonProperty("exceeded_retries")]
        public bool ExceededRetries { get; set; }

        [JsonProperty("file_urls")]
        public List<string> FileUrls { get; set; }

        [JsonProperty("error_messages")]
        public List<string> ErrorMessages { get; set; }

        [JsonProperty("stage")]
        public string Stage { get; set; }

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        [Obsolete("The API no longer returns this field; it is always the default value.")]
        [JsonProperty("email_from")]
        public string EmailFrom { get; set; }

        [Obsolete("The API no longer returns this field; it is always the default value.")]
        [JsonProperty("email_subject")]
        public string EmailSubject { get; set; }

        [Obsolete("The API no longer returns this field; it is always the default value.")]
        [JsonProperty("email_body")]
        public string EmailBody { get; set; }

        /// <summary>The caller-supplied ID of the operation that submitted the job (for example an ERP transaction or an email Message-ID), or null when none was given.</summary>
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }

        /// <summary>The job's priority as the API reports it (for example "Normal" or "High").</summary>
        [JsonProperty("priority")]
        public string Priority { get; set; }

        /// <summary>The number of files in the job, including files derived from the inputs (for example pages of a decomposed PDF).</summary>
        [JsonProperty("total_file_count")]
        public int TotalFileCount { get; set; }

        /// <summary>Why the job was last canceled, when it was.</summary>
        [JsonProperty("cancellation_reason")]
        public string CancellationReason { get; set; }

        /// <summary>Who last canceled the job, when it was.</summary>
        [JsonProperty("last_canceled_by")]
        public string LastCanceledBy { get; set; }

        /// <summary>When the job was last canceled, when it was.</summary>
        [JsonProperty("last_cancellation_date")]
        public DateTime? LastCancellationDate { get; set; }

        /// <summary>How many times the job has been canceled.</summary>
        [JsonProperty("cancel_count")]
        public int CancelCount { get; set; }

        /// <summary>How many times the job has been restarted.</summary>
        [JsonProperty("restart_count")]
        public int RestartCount { get; set; }

        /// <summary>How many restarts reused the files already downloaded.</summary>
        [JsonProperty("cached_restart_count")]
        public int CachedRestartCount { get; set; }

        /// <summary>How many restarts downloaded the remote files again.</summary>
        [JsonProperty("redownload_restart_count")]
        public int RedownloadRestartCount { get; set; }

        /// <summary>Who last restarted the job, when it was.</summary>
        [JsonProperty("restarted_by")]
        public string RestartedBy { get; set; }

        /// <summary>When the job was last restarted, when it was.</summary>
        [JsonProperty("restarted_at")]
        public DateTime? RestartedAt { get; set; }

        /// <summary>Whether every remote input source of the job could still be reached when it was last checked.</summary>
        [JsonProperty("remote_sources_available")]
        public bool RemoteSourcesAvailable { get; set; }

        /// <summary>The remote input URLs that could not be reached.</summary>
        [JsonProperty("unavailable_source_urls")]
        public List<string> UnavailableSourceUrls { get; set; }

        /// <summary>Whether any file of the job failed.</summary>
        [JsonProperty("has_failed_files")]
        public bool HasFailedFiles { get; set; }

        /// <summary>Whether any input source of the job is broken or missing.</summary>
        [JsonProperty("has_broken_input_sources")]
        public bool HasBrokenInputSources { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that contains the Id, UserName, Stage, and CreationDate.</returns>
        public override string ToString()
        {
            return $"{Id} - {UserName} - {Stage} - {CreationDate}";
        }
    }
}