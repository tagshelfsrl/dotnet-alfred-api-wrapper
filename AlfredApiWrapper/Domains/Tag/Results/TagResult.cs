using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag.Results
{
    /// <summary>
    /// Represents a tag as returned by the tag listing endpoint.
    /// </summary>
    public class TagResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

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
