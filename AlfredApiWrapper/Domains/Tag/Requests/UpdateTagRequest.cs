using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag.Requests
{
    /// <summary>
    /// Represents the payload for updating an existing tag. Only the fields that are set are sent, so an
    /// unset property leaves the corresponding tag value unchanged (partial update semantics).
    /// </summary>
    public class UpdateTagRequest
    {
        /// <summary>
        /// The new description. When <c>null</c>, the description is left unchanged.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Whether the tag is disabled. When <c>null</c>, the disabled state is left unchanged.
        /// </summary>
        [JsonProperty("disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disabled { get; set; }
    }
}
