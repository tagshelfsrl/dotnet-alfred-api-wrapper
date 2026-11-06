using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results
{
    /// <summary>
    /// The definition of a data point (a field Alfred extracts for a tag): its name, key and type, and the
    /// formatting and validation rules applied to its values. Data points are configured in Alfred; the API
    /// reads them.
    /// </summary>
    public class DataPointDefinitionResult
    {
        /// <summary>The data point's unique identifier. Values carry it as <c>metadata_id</c>.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>The tag the data point belongs to.</summary>
        [JsonProperty("tag_id")]
        public Guid TagId { get; set; }

        /// <summary>The data point's display name. It can be renamed; key on <see cref="FieldKey"/> instead.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>The tag's name. Empty when the definition was fetched on its own.</summary>
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        /// <summary>The data point's stable machine key.</summary>
        [JsonProperty("field_key")]
        public string FieldKey { get; set; }

        /// <summary>The data point's type (for example "String", "Decimal", "DateTime" or "Object").</summary>
        [JsonProperty("field_type")]
        public string FieldType { get; set; }

        /// <summary>Whether the data point holds personal information.</summary>
        [JsonProperty("is_pii")]
        public bool IsPii { get; set; }

        /// <summary>The display mask applied to personal information, when one is configured.</summary>
        [JsonProperty("pii_mask")]
        public string PiiMask { get; set; }

        /// <summary>For a structured (object) data point, the schema of its value; null otherwise.</summary>
        [JsonProperty("structured_schema")]
        public JToken StructuredSchema { get; set; }

        /// <summary>The formatting rules applied to the data point's values, or null when none are configured.</summary>
        [JsonProperty("formatters")]
        public JToken Formatters { get; set; }

        /// <summary>The validation rules applied to the data point's values, or null when none are configured.</summary>
        [JsonProperty("validators")]
        public JToken Validators { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that contains the name, field key and type.</returns>
        public override string ToString()
        {
            return $"{Name} ({FieldKey}, {FieldType})";
        }
    }
}
