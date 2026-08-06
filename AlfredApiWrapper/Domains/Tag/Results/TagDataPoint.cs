using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag.Results
{
    /// <summary>
    /// Represents a data point (metadata field) configured on a tag, as returned by the tag detail endpoint.
    /// <para>
    /// By default the tag detail endpoint returns only identity fields (<see cref="Id"/>, <see cref="Name"/>,
    /// <see cref="FieldKey"/>). The remaining configuration fields are populated only when the detail is
    /// requested with the <c>datapoints</c> expansion; otherwise they are left at their defaults.
    /// </para>
    /// </summary>
    public class TagDataPoint
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("field_key")]
        public string FieldKey { get; set; }

        [JsonProperty("display_text")]
        public string DisplayText { get; set; }

        [JsonProperty("data_type")]
        public string DataType { get; set; }

        [JsonProperty("field_type")]
        public string FieldType { get; set; }

        /// <summary>
        /// Indicates the source of the field's value (for example, whether it is fed by the tag's visual
        /// extraction rather than text extraction).
        /// </summary>
        [JsonProperty("value_source")]
        public string ValueSource { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("read_only")]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string containing the data point's FieldKey and DataType.</returns>
        public override string ToString()
        {
            return $"{FieldKey} ({DataType})";
        }
    }
}
