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
    }
}