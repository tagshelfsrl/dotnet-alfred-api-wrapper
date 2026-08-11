using System;

namespace TagShelf.Alfred.ApiWrapper.Enumerations
{
    /// <summary>
    /// Optional expansions for the tag detail endpoint. The endpoint is slim by default; each flag opts in
    /// to an additional, more expensive section of the response.
    /// </summary>
    [Flags]
    public enum TagExpand
    {
        /// <summary>No expansions: identity fields plus each data point's id, name and field key only.</summary>
        None = 0,

        /// <summary>Return the full data point configuration (data types, value sources, flags) including hidden ones.</summary>
        DataPoints = 1,

        /// <summary>Return the tag's visual-extraction configuration.</summary>
        VisualExtraction = 2,

        /// <summary>Return everything: both the full data point configuration and the visual-extraction configuration.</summary>
        All = DataPoints | VisualExtraction
    }
}
