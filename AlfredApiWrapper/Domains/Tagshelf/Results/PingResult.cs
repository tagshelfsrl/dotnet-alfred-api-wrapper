using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tagshelf.Results
{
    /// <summary>
    /// Represents the response from the /api/tagshelf/ping endpoint.
    /// </summary>
    public class PingResult
    {
        /// <summary>
        /// Gets or sets the pong response which contains an email in this context.
        /// </summary>
        [JsonProperty("pong")]
        public string Pong { get; set; }

        /// <summary>
        /// Returns a string that represents the current object, formatted to include relevant user information.
        /// </summary>
        /// <returns>A string that contains the pong response.</returns>
        public override string ToString()
        {
            return $"Pong: {Pong}";
        }
    }
}