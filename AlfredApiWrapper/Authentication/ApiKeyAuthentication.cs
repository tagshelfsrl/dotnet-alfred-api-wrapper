using System;
using System.Threading.Tasks;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Utilities;

namespace TagShelf.Alfred.ApiWrapper.Authentication
{
    /// <summary>
    /// Handles authentication using an API key.
    /// </summary>
    public class ApiKeyAuthentication
    {
        private readonly string _apiKey;
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the ApiKeyAuthentication class with a provided API key.
        /// </summary>
        /// <param name="apiKey">The API key used for authentication.</param>
        /// <param name="apiClient">The HttpApiClient instance used for making HTTP requests.</param>
        public ApiKeyAuthentication(string apiKey, HttpApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("API key must be provided.", nameof(apiKey));
            }
            _apiKey = apiKey;

            // Immediately configure the HTTP client with the API key
            ConfigureApiClient();
        }

        /// <summary>
        /// Creates an instance of ApiKeyAuthentication using an API key from an environment variable.
        /// </summary>
        /// <param name="apiClient">The HttpApiClient instance used for making HTTP requests.</param>
        /// <returns>An instance of ApiKeyAuthentication configured with an API key from an environment variable.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the environment variable does not contain a valid API key.</exception>
        public static ApiKeyAuthentication FromEnvironmentVariable(HttpApiClient apiClient)
        {
            var apiKey = EnvironmentHelper.GetEnvironmentVariable("ALFRED_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("API key environment variable 'ALFRED_API_KEY' not found or empty.");
            }
            return new ApiKeyAuthentication(apiKey, apiClient);
        }

        /// <summary>
        /// Asynchronously configures the ApiClient with the necessary authentication header.
        /// This method currently completes immediately as the API key is set in the constructor.
        /// </summary>
        public Task AuthenticateAsync()
        {
            // Configuration is done in the constructor; thus, this method completes immediately.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Configures the API client with the API key authentication header.
        /// </summary>
        private void ConfigureApiClient()
        {
            _apiClient.AddOrUpdateHeader("X-TagshelfAPI-Key", _apiKey);
        }
    }
}
