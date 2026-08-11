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

        /// <summary>
        /// The stable key of the metadata field (data point), suitable for mapping onto your own model.
        /// </summary>
        [JsonProperty("field_key")]
        public string FieldKey { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("classification_score")]
        public double ClassificationScore { get; set; }

        /// <summary>
        /// The normalized form of <see cref="Value"/>, when the platform produced one.
        /// </summary>
        [JsonProperty("normalized_value")]
        public string NormalizedValue { get; set; }

        /// <summary>
        /// The validation outcome for this data point, or <c>null</c> when no validation ran for it.
        /// </summary>
        [JsonProperty("validation")]
        public DataPointValidation Validation { get; set; }
    }
}
