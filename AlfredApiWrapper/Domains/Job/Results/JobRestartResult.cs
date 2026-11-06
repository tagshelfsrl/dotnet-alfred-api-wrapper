using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    /// <summary>
    /// The outcome of restarting a job.
    /// <para>
    /// A restart the server refuses (for example because a remote source has gone missing, answered
    /// 409, or a file type the tenant's pipeline does not accept, answered 400) is raised as an
    /// <see cref="Core.ApiException"/> whose <c>ResponseContent</c> carries the reason.
    /// </para>
    /// </summary>
    public class JobRestartResult
    {
        /// <summary>Whether the job was restarted.</summary>
        [JsonProperty("restarted")]
        public bool Restarted { get; set; }
    }
}
