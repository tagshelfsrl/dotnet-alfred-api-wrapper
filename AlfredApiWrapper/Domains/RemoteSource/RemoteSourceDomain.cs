using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.RemoteSource.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.RemoteSource.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains.RemoteSource
{
    /// <summary>
    /// Encapsulates remote sources for the Alfred API: cloud storage locations (Amazon S3, DigitalOcean Spaces,
    /// Azure Blob Storage) that uploads can read files from.
    /// </summary>
    public class RemoteSourceDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSourceDomain"/> class.
        /// </summary>
        /// <param name="apiClient">The HTTP client for API communication.</param>
        public RemoteSourceDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Lists the tenant's remote sources.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the sources.</returns>
        public async Task<List<RemoteSourceResult>> GetAllAsync()
        {
            return await _apiClient.GetAsync<List<RemoteSourceResult>>("api/source/list").ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a remote source.
        /// </summary>
        /// <param name="request">The source's name, provider and credentials.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the new source's ID.</returns>
        public async Task<CreateRemoteSourceResult> CreateAsync(CreateRemoteSourceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.SourceName))
            {
                throw new ArgumentException("A source name is required.", nameof(request));
            }

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            return await _apiClient.PostAsync<CreateRemoteSourceResult>("api/source/create", content).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a remote source by its name. Throws an <see cref="ApiException"/> with status 404 when no
        /// source has that name.
        /// </summary>
        /// <param name="sourceName">The source's name.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RemoveAsync(string sourceName)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                throw new ArgumentException("A source name is required.", nameof(sourceName));
            }

            await _apiClient.DeleteAsync<object>($"api/source/remove/{Uri.EscapeDataString(sourceName)}").ConfigureAwait(false);
        }
    }
}
