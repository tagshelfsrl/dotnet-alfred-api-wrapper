using System;
using System.Globalization;
using System.Threading.Tasks;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Search.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Search.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains.Search
{
    /// <summary>
    /// Encapsulates document search for the Alfred API.
    /// </summary>
    public class SearchDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchDomain"/> class.
        /// </summary>
        /// <param name="apiClient">The HTTP client for API communication.</param>
        public SearchDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Searches document content within the tags the caller can see, optionally narrowed to one tag.
        /// </summary>
        /// <param name="request">The search text, an optional tag, and paging.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested page of matches and the total count.</returns>
        public async Task<PagedResult<SearchResult>> BasicAsync(SearchRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                throw new ArgumentException("A search query is required.", nameof(request));
            }

            var uri = "api/search/basic" + QueryString.Build(
                QueryString.Param("query", request.Query),
                QueryString.Param("tagId", request.TagId?.ToString()),
                QueryString.Param("tagName", request.TagName),
                QueryString.Param("page", request.Page.ToString(CultureInfo.InvariantCulture)),
                QueryString.Param("pageSize", request.PageSize.ToString(CultureInfo.InvariantCulture)),
                QueryString.Flag("fuzzy", request.Fuzzy));
            return await _apiClient.GetAsync<PagedResult<SearchResult>>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Searches document content across every file the caller can see.
        /// </summary>
        /// <param name="query">The text to match against document content.</param>
        /// <param name="page">The 1-based page number. Defaults to 1.</param>
        /// <param name="pageSize">The number of results per page. Defaults to 50.</param>
        /// <param name="fuzzy">When true, approximate matches are included. Defaults to false.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested page of matches and the total count.</returns>
        public async Task<PagedResult<SearchResult>> UniversalAsync(string query, int page = 1, int pageSize = 50, bool fuzzy = false)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("A search query is required.", nameof(query));
            }

            var uri = "api/search/universal" + QueryString.Build(
                QueryString.Param("query", query),
                QueryString.Param("page", page.ToString(CultureInfo.InvariantCulture)),
                QueryString.Param("pageSize", pageSize.ToString(CultureInfo.InvariantCulture)),
                QueryString.Flag("fuzzy", fuzzy));
            return await _apiClient.GetAsync<PagedResult<SearchResult>>(uri).ConfigureAwait(false);
        }
    }
}
