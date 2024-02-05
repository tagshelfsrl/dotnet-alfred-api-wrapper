using TagShelf.Alfred.ApiWrapper.Domains.DataPoint;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession;
using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.Job;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf;

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

        /// <summary>
        /// Provides access to Tagshelf-related operations.
        /// </summary>
        TagshelfDomain Tagshelf { get; }

        /// <summary>
        /// Provides access to deferred session-related operations.
        /// </summary>
        DeferredSessionDomain DeferredSession { get; }

        /// <summary>
        /// Provides access to data point-related operations.
        /// </summary>
        DataPointDomain DataPoint { get; }
    }
}
