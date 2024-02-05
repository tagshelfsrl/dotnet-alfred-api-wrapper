using Newtonsoft.Json;
using System;

namespace TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results
{
    public class DataPointResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("file_log_id")]
        public Guid FileLogId { get; set; }

        [JsonProperty("metadata_id")]
        public Guid MetadataId { get; set; }

        [JsonProperty("metadata_name")]
        public string MetadataName { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("classification_score")]
        public double ClassificationScore { get; set; }
    }
}