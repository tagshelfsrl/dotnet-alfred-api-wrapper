using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.Job;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Defines the interface for interacting with the Alfred API.
    /// </summary>
    public interface IAlfred
    {
        /// <summary>
        /// Provides access to job-related operations.
        /// </summary>
        JobDomain Job { get; }

        /// <summary>
        /// Provides access to file-related operations.
        /// </summary>
        FileDomain File { get; }
    }
}
