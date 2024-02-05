using System.IO;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    public class FileDownloadResult
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public Stream FileContents { get; set; }
    }
}