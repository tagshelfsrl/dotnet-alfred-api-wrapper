using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains.DeferredSession
{
    /// <summary>
    /// Represents the domain-specific operations related to Deferred Sessions in Alfred.
    /// </summary>
    public class DeferredSessionDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the DeferredSessionDomain class.
        /// </summary>
        /// <param name="apiClient">The HTTP client used for making API requests.</param>
        public DeferredSessionDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Creates a new deferred session asynchronously.
        /// </summary>
        /// <returns>The result of the session creation, including the session ID.</returns>
        public async Task<CreateSessionResult> CreateAsync()
        {            
            return await _apiClient.PostAsync<CreateSessionResult>("api/deferred/create").ConfigureAwait(false);            
        }


        /// <summary>
        /// Retrieves details of a specified deferred session asynchronously.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session.</param>
        /// <returns>The detailed information about the session.</returns>
        public async Task<SessionDetailResult> GetAsync(Guid sessionId)
        {
            return await _apiClient.GetAsync<SessionDetailResult>($"api/deferred/detail/{sessionId}").ConfigureAwait(false);
        }

    }
}
