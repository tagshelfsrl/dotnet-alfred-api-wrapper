using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    public class FileUploadResult
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string that contains the job ID.</returns>
        public override string ToString()
        {
            return $"Job ID: {JobId}";
        }
    }
}