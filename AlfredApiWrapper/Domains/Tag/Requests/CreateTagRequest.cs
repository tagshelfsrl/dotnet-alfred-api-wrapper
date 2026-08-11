using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag.Requests
{
    /// <summary>
    /// Represents the payload for creating a new tag.
    /// </summary>
    public class CreateTagRequest
    {
        /// <summary>
        /// The tag name. Required.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// An optional description. Tags without a description classify poorly, so the API returns a
        /// warning when one is omitted.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
