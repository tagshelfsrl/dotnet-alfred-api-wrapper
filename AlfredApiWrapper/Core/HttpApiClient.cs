using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Encapsulates HTTP communication logic using HttpClient.
    /// </summary>
    public class HttpApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientIdentifier;

        /// <summary>
        /// Initializes a new instance of the HttpApiClient class.
        /// </summary>
        public HttpApiClient()
        {
            _httpClient = new HttpClient();
            _clientIdentifier = Guid.NewGuid().ToString(); // Unique identifier for this client instance
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"dotnet-alfred-api-wrapper/{_clientIdentifier}");
        }

        /// <summary>
        /// Sets the base address of the URI used by HttpClient instances.
        /// </summary>
        /// <param name="baseAddress">The base address of the URI.</param>
        public void SetBaseAddress(string baseAddress)
        {
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        /// <summary>
        /// Allows external entities to add or update an HTTP header for subsequent requests.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        public void AddOrUpdateHeader(string name, string value)
        {
            if (_httpClient.DefaultRequestHeaders.Contains(name))
            {
                _httpClient.DefaultRequestHeaders.Remove(name);
            }
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The HTTP response message.</returns>
        public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
        {
            return await _httpClient.PostAsync(uri, content);
        }

        /// <summary>
        /// Sends a GET request to the specified Uri and returns the response body as a string.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
        public async Task<string> GetAsync(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}