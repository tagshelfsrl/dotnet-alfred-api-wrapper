using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains.DataPoint
{
    public class DataPointDomain
    {
        private readonly HttpApiClient _apiClient;

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
    }
}