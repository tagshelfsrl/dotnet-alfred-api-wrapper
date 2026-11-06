using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    /// <summary>
    /// Whether a job's remote input sources (URLs and remote storage files) can still be reached.
    /// </summary>
    public class JobSourcesCheckResult
    {
        /// <summary>True when every remote source of the job can be reached.</summary>
        [JsonProperty("available")]
        public bool Available { get; set; }

        /// <summary>The sources that could not be reached.</summary>
        [JsonProperty("unavailable_urls")]
        public List<string> UnavailableUrls { get; set; }
    }
}
