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
            string responseContent = await _apiClient.GetAsync("/api/tagshelf/status");
            return JsonConvert.DeserializeObject<TagshelfStatusResult>(responseContent);
        }

        /// <summary>
        /// Gets information about the current user based on the provided authentication.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current user's information.</returns>
        public async Task<WhoAmIResult> WhoAmIAsync()
        {
            string responseContent = await _apiClient.GetAsync("/api/tagshelf/who-am-i");
            return JsonConvert.DeserializeObject<WhoAmIResult>(responseContent);
        }

        /// <summary>
        /// Performs a basic connectivity test to the Tagshelf platform.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ping response.</returns>
        public async Task<PingResult> PingAsync()
        {
            string responseContent = await _apiClient.GetAsync("/api/tagshelf/ping");
            return JsonConvert.DeserializeObject<PingResult>(responseContent);
        }
    }
}
