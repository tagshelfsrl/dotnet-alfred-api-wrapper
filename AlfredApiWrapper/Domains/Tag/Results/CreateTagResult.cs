using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag.Results
{
    /// <summary>
    /// Represents the result of creating a tag.
    /// </summary>
    public class CreateTagResult
    {
        /// <summary>
        /// The identifier of the newly created tag.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Non-fatal warnings raised during creation (for example, when the tag was created without a description).
        /// </summary>
        [JsonProperty("warnings")]
        public List<string> Warnings { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string containing the created tag Id.</returns>
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
