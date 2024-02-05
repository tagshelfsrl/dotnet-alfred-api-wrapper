using System.Collections.Generic;
using Newtonsoft.Json;

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

        [JsonProperty("propagate_metadata")]
        public bool? PropagateMetadata { get; set; }

        [JsonProperty("parent_file_prefix")]
        public string ParentFilePrefix { get; set; }
    }
}