using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results
{
    /// <summary>
    /// The validation outcome for a data point value: the overall status, when it ran, the individual
    /// validator results, and a roll-up summary.
    /// </summary>
    /// <remarks>
    /// Unlike the surrounding data point fields (which are snake_case), the validation payload is emitted
    /// in PascalCase; the property mappings below reflect that exactly.
    /// </remarks>
    public class DataPointValidation
    {
        /// <summary>The overall validation status (for example, <c>passed</c> or <c>failed</c>).</summary>
        [JsonProperty("Status")]
        public string Status { get; set; }

        /// <summary>When the validation ran (UTC).</summary>
        [JsonProperty("ExecutedAt")]
        public DateTime ExecutedAt { get; set; }

        /// <summary>Total time the validation took, in milliseconds.</summary>
        [JsonProperty("DurationMs")]
        public long DurationMs { get; set; }

        /// <summary>The result of each individual validator that ran.</summary>
        [JsonProperty("Results")]
        public List<DataPointValidationResult> Results { get; set; }

        /// <summary>The roll-up counts across all validators.</summary>
        [JsonProperty("Summary")]
        public DataPointValidationSummary Summary { get; set; }

        /// <summary>Whether the data point must pass validation for the file to complete.</summary>
        [JsonProperty("MustPassValidation")]
        public bool MustPassValidation { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string containing the status and the summary counts.</returns>
        public override string ToString()
        {
            return $"{Status} ({Summary})";
        }
    }
}
