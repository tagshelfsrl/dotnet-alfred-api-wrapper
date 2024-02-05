using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tagshelf.Results
{
    public class TagshelfStatusResult
    {
        /// <summary>
        /// Gets or sets the API version of the Tagshelf platform.
        /// </summary>
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the database status of the Tagshelf platform.
        /// </summary>
        [JsonProperty("db_status")]
        public string DbStatus { get; set; }

        /// <summary>
        /// Returns a string that represents the current object, formatted to include relevant user information.
        /// </summary>
        /// <returns>A string that contains the user's UserName, Name, LastName, Roles, Status, and CompanyName.</returns>
        public override string ToString()
        {
            return $"ApiVersion: {ApiVersion}, DbStatus: {DbStatus}";
        }
    }
}