using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Requests
{
    public class CreateJobRequest
    {
        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }

        [JsonProperty("propagate_metadata")]
        public bool? PropagateMetadata { get; set; }

        [JsonProperty("merge")]
        public bool? Merge { get; set; }

        [JsonProperty("decompose")]
        public bool? Decompose { get; set; }

        [JsonProperty("metadata")]
        public string Metadata { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; } = "library";

        [JsonProperty("parent_file_prefix")]
        public string ParentFilePrefix { get; set; }

        [JsonProperty("page_rotation")]
        public int? PageRotation { get; set; }

        [JsonProperty("container")]
        public string Container { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_names")]
        public List<string> FileNames { get; set; }
        
        /// <summary>
        /// The job's priority. Leave null to use the tenant's configured default priority; a value set
        /// here is ignored when the tenant forces its default.
        /// </summary>
        [JsonProperty("priority")]
        public JobPriority? JobPriority { get; set; }

        /// <summary>
        /// An optional ID of the operation that submitted the job in your system (for example an ERP
        /// transaction or an email Message-ID), at most 512 characters. It is returned on the job and can
        /// be used to find the job with <see cref="JobDomain.GetAllAsync"/>. Alfred does not deduplicate on it.
        /// </summary>
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }

    }
}