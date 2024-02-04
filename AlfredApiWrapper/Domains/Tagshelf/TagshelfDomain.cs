using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains.Tagshelf
{
    /// <summary>
    /// Represents the domain-specific operations related to Tagshelf functionalities.
    /// </summary>
    public class TagshelfDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the TagshelfDomain class.
        /// </summary>
        /// <param name="apiClient">The HTTP client used for making API requests.</param>
        public TagshelfDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new System.ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Gets the operational status of the Tagshelf platform.
        /// </summary>
        /// <returns>A task that, when completed, returns the operational status of the platform.</returns>
        public async Task<TagshelfStatusResult> StatusAsync()
        {
            return await _apiClient.GetAsync<TagshelfStatusResult>("/api/tagshelf/status").ConfigureAwait(false);            
        }

        /// <summary>
        /// Gets information about the current user based on the provided authentication.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current user's information.</returns>
        public async Task<WhoAmIResult> WhoAmIAsync()
        {
            return await _apiClient.GetAsync<WhoAmIResult>("/api/tagshelf/who-am-i").ConfigureAwait(false);            
        }

        /// <summary>
        /// Performs a basic connectivity test to the Tagshelf platform.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ping response.</returns>
        public async Task<PingResult> PingAsync()
        {
            return await _apiClient.GetAsync<PingResult>("/api/tagshelf/ping").ConfigureAwait(false);            
        }
    }
}
