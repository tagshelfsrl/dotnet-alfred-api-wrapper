using System.Collections.Generic;
using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// A page of results returned by endpoints that support opt-in paging, carrying the rows for the
    /// requested page alongside the total count and the page coordinates.
    /// </summary>
    /// <typeparam name="T">The type of the items in the page.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>The rows for the requested page.</summary>
        [JsonProperty("result")]
        public List<T> Result { get; set; }

        /// <summary>The total number of rows across all pages.</summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        /// <summary>The 1-based page number this result represents.</summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>The number of rows per page.</summary>
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
    }
}
