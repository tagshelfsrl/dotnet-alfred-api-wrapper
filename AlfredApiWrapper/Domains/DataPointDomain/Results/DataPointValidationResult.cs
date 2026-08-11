using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results
{
    /// <summary>
    /// The result of a single validator run against a data point value.
    /// </summary>
    public class DataPointValidationResult
    {
        /// <summary>The identifier of the validator that produced this result.</summary>
        [JsonProperty("ValidatorId")]
        public string ValidatorId { get; set; }

        /// <summary>The validator type (for example, <c>required</c>, <c>checksum</c>, or <c>custom</c>).</summary>
        [JsonProperty("Type")]
        public string Type { get; set; }

        /// <summary>Whether the validator passed.</summary>
        [JsonProperty("Passed")]
        public bool Passed { get; set; }

        /// <summary>Whether the validator was skipped.</summary>
        [JsonProperty("Skipped")]
        public bool Skipped { get; set; }

        /// <summary>The severity assigned to a failure of this validator.</summary>
        [JsonProperty("Severity")]
        public int Severity { get; set; }

        /// <summary>Whether a failure of this validator blocks file completion.</summary>
        [JsonProperty("BlocksCompletion")]
        public bool BlocksCompletion { get; set; }

        /// <summary>An optional human-readable message describing the outcome.</summary>
        [JsonProperty("Message")]
        public string Message { get; set; }

        /// <summary>
        /// Optional validator-specific metadata as raw JSON. The shape depends on the validator
        /// (for example, a custom validator may include the values it compared), so it is exposed as a
        /// <see cref="JObject"/> rather than a fixed type.
        /// </summary>
        [JsonProperty("Metadata")]
        public JObject Metadata { get; set; }

        /// <summary>The time this validator took, in milliseconds.</summary>
        [JsonProperty("DurationMs")]
        public long DurationMs { get; set; }

        /// <summary>The runtime that executed the validator (for example, <c>python</c>), when applicable.</summary>
        [JsonProperty("Runtime")]
        public string Runtime { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string containing the validator type and its pass/skip outcome.</returns>
        public override string ToString()
        {
            var outcome = Skipped ? "skipped" : (Passed ? "passed" : "failed");
            return $"{Type}: {outcome}";
        }
    }
}
