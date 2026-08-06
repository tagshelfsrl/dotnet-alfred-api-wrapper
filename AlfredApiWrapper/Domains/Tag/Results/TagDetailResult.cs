using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag.Results
{
    /// <summary>
    /// Represents the detailed information about a tag, including its configured data points and
    /// visual-extraction setup.
    /// </summary>
    public class TagDetailResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("company_id")]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// The data points (metadata fields) configured on the tag. By default each carries only its
        /// identity (id, name, field key) and hidden data points are excluded; request the
        /// <see cref="Enumerations.TagExpand.DataPoints"/> expansion for the full configuration including
        /// hidden data points.
        /// </summary>
        [JsonProperty("datapoints")]
        public List<TagDataPoint> DataPoints { get; set; }

        /// <summary>
        /// The tag's visual-extraction configuration as raw JSON. Only present when the detail is requested
        /// with the <see cref="Enumerations.TagExpand.VisualExtraction"/> expansion; otherwise <c>null</c>.
        /// Typed as a <see cref="JToken"/> because the shape is configuration-specific.
        /// </summary>
        [JsonProperty("visual_extraction")]
        public JToken VisualExtraction { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string containing the tag's Name and Id.</returns>
        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
