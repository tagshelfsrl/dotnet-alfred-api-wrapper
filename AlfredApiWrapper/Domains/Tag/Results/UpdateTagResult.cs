using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag.Results
{
    /// <summary>
    /// Represents the result of updating a tag.
    /// </summary>
    public class UpdateTagResult
    {
        /// <summary>
        /// The identifier of the updated tag.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string containing the updated tag Id.</returns>
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
