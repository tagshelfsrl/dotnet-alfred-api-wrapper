using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        /// <summary>A description of the file's content, when one was generated.</summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>How the file entered Alfred (for example "original" or "derived").</summary>
        [JsonProperty("input_type")]
        public string InputType { get; set; }

        /// <summary>How data was extracted from the file (for example "none" or "multimodal").</summary>
        [JsonProperty("extraction_type")]
        public string ExtractionType { get; set; }

        /// <summary>Whether the file's extracted data duplicates another file's.</summary>
        [JsonProperty("is_data_duplicate")]
        public bool IsDataDuplicate { get; set; }

        /// <summary>Whether the file duplicates another file by its data point values.</summary>
        [JsonProperty("is_duplicate_by_values")]
        public bool IsDuplicateByValues { get; set; }

        /// <summary>
        /// The file's own metadata as JSON (usually an object), or null when it has none. Read it with
        /// <c>Metadata?["key"]</c> or <c>Metadata?.ToObject&lt;T&gt;()</c>.
        /// </summary>
        [JsonProperty("metadata")]
        public JToken Metadata { get; set; }

        /// <summary>The metadata that applies to the file, including what it inherits from its job, as JSON text.</summary>
        [JsonProperty("effective_metadata")]
        public string EffectiveMetadata { get; set; }

        /// <summary>The file's position within its job.</summary>
        [JsonProperty("job_order_index")]
        public int JobOrderIndex { get; set; }

        /// <summary>The number of pages in the file.</summary>
        [JsonProperty("page_count")]
        public int PageCount { get; set; }

        /// <summary>For a page split from a larger document, its page number in that document.</summary>
        [JsonProperty("page_number")]
        public int PageNumber { get; set; }

        /// <summary>Who the file is assigned to for review; empty when nobody is.</summary>
        [JsonProperty("assigned_to")]
        public string AssignedTo { get; set; }

        /// <summary>When the file was assigned for review (UTC), or null when it was not.</summary>
        [JsonProperty("assignment_date")]
        public DateTime? AssignmentDate { get; set; }

        /// <summary>When the file was validated (UTC), or null when it has not been.</summary>
        [JsonProperty("validation_date")]
        public DateTime? ValidationDate { get; set; }

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