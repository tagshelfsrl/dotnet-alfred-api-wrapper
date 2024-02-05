using Newtonsoft.Json;
using System;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Results
{
    public class FileDetailResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("update_date")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_name_without_extension")]
        public string FileNameWithoutExtension { get; set; }

        [JsonProperty("blob_name")]
        public string BlobName { get; set; }

        [JsonProperty("blob_url")]
        public string BlobUrl { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("md5_hash")]
        public string Md5Hash { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("should_be_classified")]
        public bool ShouldBeClassified { get; set; }

        [JsonProperty("classifier")]
        public string Classifier { get; set; }

        [JsonProperty("classification_score")]
        public double ClassificationScore { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("is_duplicate")]
        public bool IsDuplicate { get; set; }

        [JsonProperty("duplicate_origin_id")]
        public Guid? DuplicateOriginId { get; set; }

        [JsonProperty("tag_id")]
        public Guid? TagId { get; set; }

        [JsonProperty("is_parent")]
        public bool IsParent { get; set; }

        [JsonProperty("parent_id")]
        public Guid? ParentId { get; set; }

        [JsonProperty("deferred_session_id")]
        public Guid? DeferredSessionId { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty("file_size")]
        public long FileSize { get; set; }

        [JsonProperty("proposed_tag_id")]
        public Guid? ProposedTagId { get; set; }

        [JsonProperty("proposed_tag_variance")]
        public double ProposedTagVariance { get; set; }

        [JsonProperty("classification_score_above_deviation")]
        public bool ClassificationScoreAboveDeviation { get; set; }

        [JsonProperty("confirmed_tag_id")]
        public Guid? ConfirmedTagId { get; set; }

        [JsonProperty("confirmed_by")]
        public string ConfirmedBy { get; set; }

        [JsonProperty("manual_classification")]
        public bool ManualClassification { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that contains the file's ID, file name, status, content type, file size, and creation date.
        /// </returns>
        public override string ToString()
        {
            return $"Id: {Id}, FileName: {FileName}, Status: {Status}, ContentType: {ContentType}, FileSize: {FileSize} bytes, CreationDate: {CreationDate}";
        }
    }
}