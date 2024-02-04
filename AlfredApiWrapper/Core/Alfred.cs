using System;
using TagShelf.Alfred.ApiWrapper.Authentication;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Provides a single interface for API interactions, encapsulating HTTP request handling.
    /// This class ensures that an API key is provided for authentication, either directly or through an environment variable.
    /// </summary>
    public class Alfred
    {
        private readonly HttpApiClient _apiClient;
        private readonly ApiKeyAuthentication _apiKeyAuth;

        /// <summary>
        /// Initializes a new instance of the Alfred class using only an API key, defaulting the environment to Production.
        /// </summary>
        /// <param name="apiKey">The API key for authentication. If null, the constructor attempts to fetch it from the ALFRED_API_KEY environment variable.</param>
        public Alfred(string apiKey)
            : this(apiKey, EnvironmentType.Production)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Alfred class with both API key and environment specified.
        /// </summary>
        /// <param name="apiKey">API key for authentication. If null, attempts to fetch from ALFRED_API_KEY environment variable.</param>
        /// <param name="environment">Environment selection.</param>
        public Alfred(string apiKey, EnvironmentType environment)
        {
            _apiClient = new HttpApiClient();
            _apiKeyAuth = new ApiKeyAuthentication(apiKey);

            string baseUrl = GetBaseUrlByEnvironment(environment);
            _apiClient.SetBaseAddress(baseUrl);

            _apiKeyAuth.AddAuthenticationHeader(_apiClient);
        }

        /// <summary>
        /// Initializes a new instance of the Alfred class with OAuth authentication.
        /// </summary>
        /// <param name="username">The user's username for OAuth authentication.</param>
        /// <param name="password">The user's password for OAuth authentication.</param>
        /// <param name="method">The authentication method to use. Defaults to OAuth.</param>
        /// <param name="environment">The environment to target. Defaults to Production.</param>
        public Alfred(string username, string password, AuthenticationMethod method = AuthenticationMethod.OAuth, EnvironmentType environment = EnvironmentType.Production)
        {
            _apiClient = new HttpApiClient();

            string baseUrl = GetBaseUrlByEnvironment(environment);
            _apiClient.SetBaseAddress(baseUrl);

            if (method == AuthenticationMethod.OAuth)
            {
                var oauth = new OAuthAuthentication(username, password, _apiClient);
                oauth.AuthenticateAsync().Wait(); // Caution: Blocking call. Consider using async pattern throughout.
            }
            else
            {
                // Initialize other authentication methods as needed
            }
        }

        /// <summary>
        /// Determines the base URL according to the specified environment.
        /// </summary>
        /// <param name="environment">The specified environment.</param>
        /// <returns>The base URL for the given environment.</returns>
        /// <exception cref="ArgumentException">Thrown when an unsupported environment type is specified.</exception>
        private string GetBaseUrlByEnvironment(EnvironmentType environment)
        {
            switch (environment)
            {
                case EnvironmentType.Production:
                    return "https://app.tagshelf.com";
                case EnvironmentType.Staging:
                    return "https://staging.tagshelf.com";
                default:
                    throw new ArgumentException("Unsupported environment type", nameof(environment));
            }
        }        
    }
}
