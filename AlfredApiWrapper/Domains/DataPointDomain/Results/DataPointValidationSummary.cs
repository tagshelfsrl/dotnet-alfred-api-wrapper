using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results
{
    /// <summary>
    /// A roll-up of the individual validator results for a data point.
    /// </summary>
    public class DataPointValidationSummary
    {
        /// <summary>The total number of validators that ran.</summary>
        [JsonProperty("Total")]
        public int Total { get; set; }

        /// <summary>The number of validators that passed.</summary>
        [JsonProperty("Passed")]
        public int Passed { get; set; }

        /// <summary>The number of validators that failed.</summary>
        [JsonProperty("Failed")]
        public int Failed { get; set; }

        /// <summary>The number of validators that were skipped.</summary>
        [JsonProperty("Skipped")]
        public int Skipped { get; set; }

        /// <summary>Whether any failing validator blocks file completion.</summary>
        [JsonProperty("HasBlockingFailures")]
        public bool HasBlockingFailures { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string summarizing the pass/fail/skip counts.</returns>
        public override string ToString()
        {
            return $"{Passed}/{Total} passed, {Failed} failed, {Skipped} skipped";
        }
    }
}
