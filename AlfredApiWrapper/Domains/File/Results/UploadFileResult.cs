using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    public class UploadFileResult
    {
        [JsonProperty("file_id")]
        public Guid FileId { get; set; }


        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string that contains the file ID.</returns>
        public override string ToString()
        {
            return $"File ID: {FileId}";
        }
    }
}