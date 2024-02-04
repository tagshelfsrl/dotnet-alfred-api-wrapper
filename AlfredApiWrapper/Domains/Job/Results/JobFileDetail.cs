
using System;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Results
{
    public class JobFileDetail
    {
        [JsonProperty("id")]
        public Guid FileId { get; set; }

        [JsonProperty("creation_date")]
        public DateTime FileCreationDate { get; set; }

        [JsonProperty("update_date")]
        public DateTime FileUpdateDate { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("is_parent")]
        public bool IsParent {get;set;}
        
        [JsonProperty("is_children")]
        public bool IsChildren {get;set;}

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that contains the file's FileName, TagName, and Status.</returns>
        public override string ToString()
        {
            return $"{FileName} - {TagName} - {Status}";
        }
    }
}