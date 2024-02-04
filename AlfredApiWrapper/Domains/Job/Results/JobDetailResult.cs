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

        [JsonProperty("has_job_request_info")]
        public bool HasJobRequestInfo { get; set; }

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

        [JsonProperty("email_from")]
        public string EmailFrom { get; set; }

        [JsonProperty("email_subject")]
        public string EmailSubject { get; set; }

        [JsonProperty("email_body")]
        public string EmailBody { get; set; }

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