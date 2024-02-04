using System.Threading.Tasks;
using TagShelf.Alfred.ApiWrapper.Authentication;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Factory class for creating instances of the Alfred class with various authentication strategies.
    /// </summary>
    public static class AlfredFactory
    {
        /// <summary>
        /// Creates an Alfred instance using OAuth authentication with provided user credentials.
        /// </summary>
        /// <param name="username">User's username for OAuth authentication.</param>
        /// <param name="password">User's password for OAuth authentication.</param>
        /// <param name="environment">The target environment for the API client. Defaults to Production.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the initialized Alfred instance.</returns>
        public static async Task<Alfred> CreateWithOAuthAsync(string username, string password, EnvironmentType environment = EnvironmentType.Production)
        {
            var alfred = new Alfred(environment);
            var oauth = new OAuthAuthentication(username, password, alfred.ApiClient);
            await oauth.AuthenticateAsync();
            return alfred;
        }

        /// <summary>
        /// Creates an Alfred instance using an API key directly provided by the caller.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="environment">The target environment for the API client. Defaults to Production.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the initialized Alfred instance.</returns>
        public static async Task<Alfred> CreateWithApiKeyAsync(string apiKey, EnvironmentType environment = EnvironmentType.Production)
        {
            var alfred = new Alfred(environment);
            var apiKeyAuth = new ApiKeyAuthentication(apiKey, alfred.ApiClient);
            await apiKeyAuth.AuthenticateAsync();
            return alfred;
        }

        /// <summary>
        /// Asynchronously creates an instance of the Alfred class configured for HMAC authentication.
        /// </summary>
        /// <param name="secretKey">The secret key used for generating the HMAC signature.</param>
        /// <param name="apiKey">The API key associated with the user, involved in HMAC signature computation.</param>
        /// <param name="environment">Specifies the environment the Alfred client will target, defaulting to Production.</param>
        /// <returns>A task that represents the asynchronous operation, returning an initialized Alfred instance configured for HMAC authentication.</returns>
        /// <remarks>
        /// This method initializes an Alfred instance with HMAC authentication, which involves computing a signature for each request based on the provided secretKey and apiKey.
        /// The actual request signing process should be integrated into the HttpApiClient's request handling, potentially using middleware or custom request handlers to inject the 'Authentication' header with the computed HMAC signature into each outgoing request.
        /// </remarks>
        public static async Task<Alfred> CreateWithHmacAsync(string secretKey, string apiKey, EnvironmentType environment = EnvironmentType.Production)
        {
            var alfred = new Alfred(environment);
            // Assuming HttpApiClient has a method to intercept and modify requests for HMAC signing
            // This is a placeholder for where you would integrate HMAC signing into the request process
            // For actual request signing, consider middleware or handlers depending on your HTTP client setup
            return alfred;
        }

        /// <summary>
        /// Creates an Alfred instance using an API key retrieved from an environment variable.
        /// </summary>
        /// <param name="environment">The target environment for the API client. Defaults to Production.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the initialized Alfred instance.</returns>
        public static async Task<Alfred> CreateWithApiKeyFromEnvironmentAsync(EnvironmentType environment = EnvironmentType.Production)
        {
            var alfred = new Alfred(environment);
            var apiKeyAuth = ApiKeyAuthentication.FromEnvironmentVariable(alfred.ApiClient);
            await apiKeyAuth.AuthenticateAsync();
            return alfred;
        }

        // Additional factory methods for other authentication strategies can be included here.
    }
}
