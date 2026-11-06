using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    public class CreateJobResult
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }

        /// <summary>The correlation ID the job was submitted with, or null when none was given.</summary>
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }

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