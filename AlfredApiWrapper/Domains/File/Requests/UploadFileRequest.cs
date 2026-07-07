using System;
using System.IO;

namespace AlfredApiWrapper.Domains.File.Requests
{
    public class UploadFileRequest
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public Guid SessionId { get; set; }
        public string Metadata { get; set; }

        /// <summary>
        /// Optional MIME content type for the file being uploaded (e.g. "application/pdf").
        /// When not provided, the type is inferred from <see cref="FileName"/>'s extension;
        /// if inference fails, "application/octet-stream" is used as fallback.
        /// </summary>
        public string ContentType { get; set; }
    }
}