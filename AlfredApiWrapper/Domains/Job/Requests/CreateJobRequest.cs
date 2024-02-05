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
    }
}