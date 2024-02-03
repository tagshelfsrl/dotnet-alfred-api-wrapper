using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    public class HttpApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        public HttpApiClient(string baseAddress)
        {
            _httpClient = new HttpClient();
            _baseAddress = baseAddress;
            _httpClient.BaseAddress = new Uri(_baseAddress);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpApiClient"/> configured for the specified environment and authentication method.
        /// </summary>
        /// <param name="apiKey">The API key used for authentication.</param>
        /// <param name="environment">The target environment of the Alfred API.</param>
        public HttpApiClient(string apiKey, EnvironmentType environment)
        {
            _httpClient = new HttpClient();

            // Determine the base address based on the specified environment
            switch (environment)
            {
                case EnvironmentType.Production:
                    _baseAddress = "https://app.tagshelf.com/";
                    break;
                case EnvironmentType.Staging:
                    _baseAddress = "https://staging.tagshelf.com/";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(environment), environment, "Unsupported environment type.");
            }

            _httpClient.BaseAddress = new Uri(_baseAddress);

            // Set the API key in the custom header for authentication
            _httpClient.DefaultRequestHeaders.Add("X-TagshelfAPI-Key", apiKey);
        }

        public void SetAuthenticationHeader(AuthenticationMethod method, string credential)
        {
            switch (method)
            {
                case AuthenticationMethod.ApiKey:
                    _httpClient.DefaultRequestHeaders.Add("X-TagshelfAPI-Key", credential);
                    break;
                case AuthenticationMethod.OAuth:
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credential);
                    break;
                case AuthenticationMethod.HMAC:
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", credential);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        // Implement PostAsync, PutAsync, DeleteAsync similarly, utilizing JSON serialization/deserialization

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}