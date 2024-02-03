using System;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job
{
    /// <summary>
    /// Represents the result of a job operation.
    /// </summary>
    public class JobResult
    {
        /// <summary>
        /// Gets or sets the ID of the job.
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Gets or sets the status of the job.
        /// </summary>
        public string Status { get; set; }

        // Include other relevant job properties as needed
    }
}
