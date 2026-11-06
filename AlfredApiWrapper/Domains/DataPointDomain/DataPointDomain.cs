using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains.DataPoint
{
    /// <summary>
    /// Encapsulates data point operations for the Alfred API: the values extracted from files and the
    /// definitions of each tag's data points.
    /// </summary>
    public class DataPointDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointDomain"/> class.
        /// </summary>
        /// <param name="apiClient">The HTTP client for API communication.</param>
        public DataPointDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Fetches a list of metadata values for a given file ID.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of DataPointResult.</returns>
        public async Task<List<DataPointResult>> GetValuesAsync(Guid fileId)
        {
            return await _apiClient.GetAsync<List<DataPointResult>>($"api/values/file/{fileId}").ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches the confirmed values of every file of a tag, one row per file.
        /// <para>
        /// Each row is a flat object. It has fixed keys (<c>file_log_id</c>, <c>file_id</c>, <c>tag_id</c>,
        /// <c>parent_id</c>, <c>tag_name</c>, <c>user_name</c>, <c>blob_name</c>, <c>blob_url</c>, <c>file_size</c>,
        /// <c>content_type</c>, <c>creation_date</c>, <c>update_date</c>) plus one key per data point: the data
        /// point's name in snake_case (for example <c>InvoiceNumber</c> becomes <c>invoice_number</c>), holding
        /// the raw confirmed value. A data point with no confirmed value is left out of that row.
        /// </para>
        /// <para>
        /// The response is not paged and can be large for a busy tag. Use <see cref="GetValuesAsync"/> for one
        /// file's values with their normalized forms and validation results.
        /// </para>
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains one row per confirmed file.</returns>
        public async Task<List<Dictionary<string, JToken>>> GetValuesByTagAsync(Guid tagId)
        {
            return await _apiClient.GetAsync<List<Dictionary<string, JToken>>>($"api/values/tag/{tagId}").ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the data point definitions of a tag: what Alfred extracts for that document type, with each data
        /// point's type and formatting and validation rules.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the tag's data point definitions.</returns>
        public async Task<List<DataPointDefinitionResult>> GetDefinitionsAsync(Guid tagId)
        {
            return await _apiClient.GetAsync<List<DataPointDefinitionResult>>($"api/metadata/tag/{tagId}").ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches one data point definition by its unique ID (the <c>metadata_id</c> on a value).
        /// </summary>
        /// <param name="dataPointId">The unique identifier of the data point.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the data point definition.</returns>
        public async Task<DataPointDefinitionResult> GetDefinitionAsync(Guid dataPointId)
        {
            return await _apiClient.GetAsync<DataPointDefinitionResult>($"api/metadata/detail/{dataPointId}").ConfigureAwait(false);
        }
    }
}
