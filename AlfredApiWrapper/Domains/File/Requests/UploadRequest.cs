using System.Collections.Generic;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace AlfredApiWrapper.Domains.File.Requests
{
    public class UploadRequest
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("urls")]
        public List<string> Urls { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("container")]
        public string Container { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_names")]
        public List<string> FileNames { get; set; }

        [JsonProperty("merge")]
        public bool Merge { get; set; } = false;

        [JsonProperty("metadata")]
        public object Metadata { get; set; }

        /// <summary>
        /// The job's priority. Leave null to use the tenant's configured default priority; a value set
        /// here is ignored when the tenant forces its default.
        /// </summary>
        [JsonProperty("priority")]
        public JobPriority? JobPriority { get; set; }

        [JsonProperty("propagate_metadata")]
        public bool? PropagateMetadata { get; set; }

        /// <summary>
        /// The prefix for the files derived from each input. Applies only when <see cref="Decompose"/> is true.
        /// </summary>
        [JsonProperty("parent_file_prefix")]
        public string ParentFilePrefix { get; set; }

        /// <summary>
        /// Splits each input into separate units of work (for example each page of a multi-page PDF).
        /// Leave null for the server default (false).
        /// </summary>
        [JsonProperty("decompose")]
        public bool? Decompose { get; set; }

        /// <summary>
        /// Rotation, in degrees (0, 90, 180 or 270), applied to decomposed pages. Applies only when
        /// <see cref="Decompose"/> is true.
        /// </summary>
        [JsonProperty("page_rotation")]
        public int? PageRotation { get; set; }

        /// <summary>
        /// The channel the job is submitted through (for example "api"). Leave null for the server default.
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// An optional ID of the operation that submitted the job in your system (for example an ERP
        /// transaction or an email Message-ID), at most 512 characters. It is returned on the job and can
        /// be used to find the job with <c>Job.GetAllAsync</c>. Alfred does not deduplicate on it.
        /// </summary>
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }
    }
}