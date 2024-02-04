using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;

namespace TagShelf.Alfred.ApiWrapper.Authentication
{
    /// <summary>
    /// Handles OAuth 2.0 authentication by exchanging user credentials for an access token.
    /// </summary>
    public class OAuthAuthentication
    {
        private readonly string _username;
        private readonly string _password;
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the OAuthAuthentication class.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="apiClient">The HttpApiClient used for making HTTP requests.</param>
        public OAuthAuthentication(string username, string password, HttpApiClient apiClient)
        {
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Asynchronously authenticates the user and sets the authorization header for subsequent requests.
        /// </summary>
        public async Task AuthenticateAsync()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", _username),
                new KeyValuePair<string, string>("password", _password),
            });
            
            try
            {
                var tokenResponse  = await _apiClient.PostAsync<TokenResponse>("/token", content);
                _apiClient.AddOrUpdateHeader("Authorization", $"Bearer {tokenResponse.AccessToken}");
            }
            catch (HttpRequestException ex)
            {                
                throw new InvalidOperationException("Failed to authenticate with OAuth.", ex);
            }
            catch (JsonException ex)
            {                
                throw new InvalidOperationException("Error parsing the authentication response.", ex);
            }
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("userName")]
            public string UserName { get; set; }
        }        
    }
}
