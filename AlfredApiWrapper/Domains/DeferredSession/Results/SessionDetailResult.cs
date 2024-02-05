using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.DeferredSession.Results
{

    /// <summary>
    /// Represents detailed information about a deferred session.
    /// </summary>
    public class SessionDetailResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty("job_id")]
        public string JobId { get; set; } // Nullable if not used in C# 7

        /// <summary>
        /// Returns a string that represents the current object, formatted to include relevant user information.
        /// </summary>
        /// <returns>A string that contains the user's Id, CreationDate, UpdateDate, Status, UserName, CompanyId, and JobId.</returns>
        public override string ToString()
        {
            return $"Id: {Id}, CreationDate: {CreationDate}, UpdateDate: {UpdateDate}, Status: {Status}, UserName: {UserName}, CompanyId: {CompanyId}, JobId: {JobId}";
        }
    }
}