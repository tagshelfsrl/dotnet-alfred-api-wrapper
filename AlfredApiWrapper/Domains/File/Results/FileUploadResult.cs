using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    public class FileUploadResult
    {
        [JsonProperty("job_id")]
        public Guid JobId { get; set; }

        /// <summary>The correlation ID the job was submitted with, or null when none was given.</summary>
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }

        /// <summary>
        /// Warnings about the submission (for example that the tenant has no enabled tags), or null when
        /// there are none. The job is still created when warnings are present.
        /// </summary>
        [JsonProperty("warnings")]
        public List<string> Warnings { get; set; }

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