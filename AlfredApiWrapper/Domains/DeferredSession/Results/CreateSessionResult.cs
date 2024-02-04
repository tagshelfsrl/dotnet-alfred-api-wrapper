using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.DeferredSession.Results
{
    /// <summary>
    /// Represents the result of creating a new deferred session.
    /// </summary>
    public class CreateSessionResult
    {
        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }

        /// <summary>
        /// Returns a string that represents the current object, formatted to include relevant user information.
        /// </summary>
        /// <returns>A string that contains the user's SessionId.</returns>
        public override string ToString()
        {
            return $"SessionId: {SessionId}";
        }
    }
}
