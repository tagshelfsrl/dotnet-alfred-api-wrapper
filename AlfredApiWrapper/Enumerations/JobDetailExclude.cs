using System;

namespace TagShelf.Alfred.ApiWrapper.Enumerations
{
    /// <summary>
    /// Sections of the job detail response that can be left out to make the response smaller and faster.
    /// </summary>
    [Flags]
    public enum JobDetailExclude
    {
        /// <summary>Return the full job detail.</summary>
        None = 0,

        /// <summary>Leave out the job's files.</summary>
        Files = 1,

        /// <summary>Leave out the job's metadata.</summary>
        Metadata = 2,

        /// <summary>Leave out the job's input file URLs.</summary>
        FileUrls = 4,

        /// <summary>Leave out the job's error messages.</summary>
        ErrorMessages = 8,

        /// <summary>Leave out every optional section; the job's identity, counts and status remain.</summary>
        All = Files | Metadata | FileUrls | ErrorMessages
    }
}
