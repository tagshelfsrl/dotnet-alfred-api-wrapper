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
