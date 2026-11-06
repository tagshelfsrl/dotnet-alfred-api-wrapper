using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TagShelf.Alfred.ApiWrapper.Domains.Search.Results
{
    /// <summary>
    /// A file that matched a search, with its tag and extracted data points.
    /// </summary>
    public class SearchResult
    {
        /// <summary>The matching file's unique identifier.</summary>
        [JsonProperty("file_id")]
        public string FileId { get; set; }

        /// <summary>The job the file belongs to.</summary>
        [JsonProperty("job_id")]
        public string JobId { get; set; }

        /// <summary>The channel the file was submitted through (for example "api" or "web").</summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>The user or application that submitted the file.</summary>
        [JsonProperty("user_name")]
        public string UserName { get; set; }

        /// <summary>The file's size, in bytes.</summary>
        [JsonProperty("length")]
        public double Length { get; set; }

        /// <summary>The file's tag.</summary>
        [JsonProperty("tag_id")]
        public Guid TagId { get; set; }

        /// <summary>The name of the file's tag.</summary>
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        /// <summary>The classification confidence for the tag, from 0 to 1.</summary>
        [JsonProperty("tag_score")]
        public double TagScore { get; set; }

        /// <summary>Who assigned the tag (for example "Alfred" for automatic classification).</summary>
        [JsonProperty("tag_source")]
        public string TagSource { get; set; }

        /// <summary>The classifier that assigned the tag.</summary>
        [JsonProperty("classifier")]
        public string Classifier { get; set; }

        /// <summary>When the file was created (UTC).</summary>
        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        /// <summary>When the file was last updated (UTC).</summary>
        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; }

        /// <summary>The file's extracted data points, or null when it has none.</summary>
        [JsonProperty("data_points")]
        public List<SearchDataPoint> DataPoints { get; set; }
    }

    /// <summary>
    /// A data point value on a search result.
    /// </summary>
    public class SearchDataPoint
    {
        /// <summary>The data point's name.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>The value in its typed JSON form (a string, number, date or object, depending on <see cref="DataType"/>).</summary>
        [JsonProperty("value")]
        public JToken Value { get; set; }

        /// <summary>The value as text.</summary>
        [JsonProperty("value_as_string")]
        public string ValueAsString { get; set; }

        /// <summary>The value's type (for example "String", "Decimal" or "DateTime").</summary>
        [JsonProperty("data_type")]
        public string DataType { get; set; }
    }
}
