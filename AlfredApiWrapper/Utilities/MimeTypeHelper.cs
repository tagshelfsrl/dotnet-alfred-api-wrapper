using System;
using System.Collections.Generic;
using System.IO;

namespace TagShelf.Alfred.ApiWrapper.Utilities
{
    /// <summary>
    /// Maps file extensions to MIME content types so that uploaded files
    /// carry a meaningful Content-Type instead of a blanket octet-stream.
    /// Alfred's processing pipeline relies on the content type to pick the
    /// correct parser (PDF, TIFF, image, …); an incorrect or generic type
    /// causes magic-byte detection to reject the file.
    /// </summary>
    public static class MimeTypeHelper
    {
        private static readonly Dictionary<string, string> ExtensionToMimeType =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".pdf",  "application/pdf" },
                { ".tif",  "image/tiff" },
                { ".tiff", "image/tiff" },
                { ".zip",  "application/zip" },
                { ".jpg",  "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png",  "image/png" },
                { ".gif",  "image/gif" },
                { ".bmp",  "image/bmp" },
            };

        /// <summary>
        /// Fallback used when no mapping is found for the given extension.
        /// </summary>
        public const string DefaultMimeType = "application/octet-stream";

        /// <summary>
        /// Returns the MIME type for <paramref name="fileName"/> based on its
        /// extension. When the extension is unknown (or missing) the value is
        /// <see cref="DefaultMimeType"/>.
        /// </summary>
        public static string GetMimeType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return DefaultMimeType;

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                return DefaultMimeType;

            return ExtensionToMimeType.TryGetValue(extension, out var mimeType)
                ? mimeType
                : DefaultMimeType;
        }
    }
}
