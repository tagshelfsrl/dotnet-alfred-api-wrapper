using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Account.Results
{
    /// <summary>
    /// An account (tenant) the caller has access to.
    /// </summary>
    public class CompanyResult
    {
        /// <summary>The account's unique identifier.</summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>The account's name.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The account name.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
