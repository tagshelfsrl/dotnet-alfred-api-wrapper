using System;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Utilities;

namespace TagShelf.Alfred.ApiWrapper.Authentication
{
    /// <summary>
    /// Handles API key authentication by managing the API key and setting it in the HTTP header.
    /// </summary>
    public class ApiKeyAuthentication
    {
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the ApiKeyAuthentication class with the specified API key.
        /// </summary>
        /// <param name="apiKey">The API key used for authentication.</param>
        public ApiKeyAuthentication(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = EnvironmentHelper.GetEnvironmentVariable("ALFRED_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException("API key must be provided either directly or via the ALFRED_API_KEY environment variable.");
                }
            }

            _apiKey = apiKey;
        }

        /// <summary>
        /// Adds the API key to the specified HttpClient's default request headers.
        /// </summary>
        /// <param name="client">The HttpClient to which the API key will be added.</param>
        public void AddAuthenticationHeader(HttpApiClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            
            client.AddOrUpdateHeader("X-TagshelfAPI-Key", _apiKey);
        }
    }
}
