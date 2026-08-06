using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tag
{
    /// <summary>
    /// Encapsulates tag-related operations for the Alfred API: listing, creating, retrieving, and updating tags.
    /// </summary>
    public class TagDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagDomain"/> class.
        /// </summary>
        /// <param name="apiClient">The HTTP client for API communication.</param>
        public TagDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Lists all tags accessible to the authenticated caller (unpaged).
        /// </summary>
        /// <param name="query">Optional case-insensitive substring match on tag name or description.</param>
        /// <param name="includeDisabled">When <c>true</c>, disabled tags are included. Defaults to <c>false</c>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of tags.</returns>
        public async Task<List<TagResult>> GetAllAsync(string query = null, bool includeDisabled = false)
        {
            var uri = "api/tags" + BuildQueryString(
                Param("query", query),
                Flag("include_disabled", includeDisabled));
            return await _apiClient.GetAsync<List<TagResult>>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists tags accessible to the authenticated caller as a single page.
        /// </summary>
        /// <param name="page">The 1-based page number. Defaults to 1.</param>
        /// <param name="pageSize">The number of rows per page. The server caps this (currently at 200). Defaults to 50.</param>
        /// <param name="query">Optional case-insensitive substring match on tag name or description.</param>
        /// <param name="includeDisabled">When <c>true</c>, disabled tags are included. Defaults to <c>false</c>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested page of tags.</returns>
        public async Task<PagedResult<TagResult>> GetPagedAsync(int page = 1, int pageSize = 50, string query = null, bool includeDisabled = false)
        {
            var uri = "api/tags" + BuildQueryString(
                Param("page", page.ToString()),
                Param("page_size", pageSize.ToString()),
                Param("query", query),
                Flag("include_disabled", includeDisabled));
            return await _apiClient.GetAsync<PagedResult<TagResult>>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="request">The tag creation details. <see cref="CreateTagRequest.Name"/> is required.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created tag's ID and any warnings.</returns>
        public async Task<CreateTagResult> CreateAsync(CreateTagRequest request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            return await _apiClient.PostAsync<CreateTagResult>("api/tag", content).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches information about a specific tag by its unique ID.
        /// <para>
        /// The response is slim by default (identity fields plus each data point's id, name and field key).
        /// Use <paramref name="expand"/> to opt in to the full data point configuration and/or the tag's
        /// visual-extraction configuration.
        /// </para>
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag.</param>
        /// <param name="expand">Optional expansions to include in the response. Defaults to <see cref="TagExpand.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the tag information.</returns>
        public async Task<TagDetailResult> GetAsync(Guid tagId, TagExpand expand = TagExpand.None)
        {
            var uri = $"api/tag/{tagId}" + BuildQueryString(Param("expand", BuildExpandValue(expand)));
            return await _apiClient.GetAsync<TagDetailResult>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates an existing tag. Only the fields set on <paramref name="request"/> are changed.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to update.</param>
        /// <param name="request">The fields to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated tag's ID.</returns>
        public async Task<UpdateTagResult> UpdateAsync(Guid tagId, UpdateTagRequest request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            return await _apiClient.PatchAsync<UpdateTagResult>($"api/tag/{tagId}", content).ConfigureAwait(false);
        }

        /// <summary>Maps <see cref="TagExpand"/> flags to the comma-separated <c>expand</c> query value, or null when none.</summary>
        internal static string BuildExpandValue(TagExpand expand)
        {
            var parts = new List<string>();
            if ((expand & TagExpand.DataPoints) != 0)
            {
                parts.Add("datapoints");
            }
            if ((expand & TagExpand.VisualExtraction) != 0)
            {
                parts.Add("visual_extraction");
            }
            return parts.Count == 0 ? null : string.Join(",", parts);
        }

        private static KeyValuePair<string, string> Param(string name, string value)
        {
            return new KeyValuePair<string, string>(name, value);
        }

        // A boolean flag is only carried when true; false is the server default, so it is omitted.
        private static KeyValuePair<string, string> Flag(string name, bool value)
        {
            return new KeyValuePair<string, string>(name, value ? "true" : null);
        }

        /// <summary>
        /// Builds a query string ("?a=b&amp;c=d") from the supplied parameters, skipping any with a
        /// null/empty value and URL-encoding the rest. Returns an empty string when nothing is set.
        /// </summary>
        internal static string BuildQueryString(params KeyValuePair<string, string>[] parameters)
        {
            var parts = new List<string>();
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter.Value))
                {
                    continue;
                }
                parts.Add(Uri.EscapeDataString(parameter.Key) + "=" + Uri.EscapeDataString(parameter.Value));
            }
            return parts.Count == 0 ? string.Empty : "?" + string.Join("&", parts);
        }
    }
}
