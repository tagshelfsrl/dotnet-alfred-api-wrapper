using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    public class CreateJobResult
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that contains the JobId.</returns>
        public override string ToString()
        {
            return JobId.ToString();
        }
    }
}