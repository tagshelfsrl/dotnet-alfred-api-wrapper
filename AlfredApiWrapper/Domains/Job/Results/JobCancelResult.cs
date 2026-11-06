using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    /// <summary>
    /// The outcome of canceling a job.
    /// </summary>
    public class JobCancelResult
    {
        /// <summary>Whether the job was canceled. False when the job could not be canceled, for example because it had already finished.</summary>
        [JsonProperty("canceled")]
        public bool Canceled { get; set; }
    }
}
