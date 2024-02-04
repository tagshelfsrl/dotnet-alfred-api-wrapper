using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Represents an error response from the API.
    /// </summary>
    public class ErrorResult
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
