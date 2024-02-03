using System;

namespace TagShelf.Alfred.ApiWrapper.Domains.File
{
    /// <summary>
    /// Represents the result of a file operation.
    /// </summary>
    public class FileResult
    {
        /// <summary>
        /// Gets or sets the ID of the file.
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        public long Size { get; set; }

        // Include other relevant file properties as needed
    }
}
