using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            // _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
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
        /// Sends a POST request to the specified Uri as an asynchronous operation and deserializes the JSON response.
        /// This method ensures that an empty content request is still sent with a Content-Type of application/json.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized result.</typeparam>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server, or null if no content is being sent.</param>
        /// <returns>A task that represents the asynchronous operation, containing the deserialized response.</returns>
        public Task<TResult> PostAsync<TResult>(string uri, HttpContent content = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                // Use an empty StringContent with application/json type if no content is provided.
                Content = content ?? new StringContent(string.Empty, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return SendAsync<TResult>(request);
        }

        /// <summary>
        /// Helper method to easily create and send a GET request.
        /// </summary>
        /// <typeparam name="TResult">The expected type of the response object.</typeparam>
        /// <param name="uri">The URI the request is sent to.</param>
        /// <returns>The deserialized response object.</returns>
        public Task<TResult> GetAsync<TResult>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return SendAsync<TResult>(request);
        }

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation and deserializes the JSON response to a specified type.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized result.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>A task that represents the asynchronous operation, containing the deserialized response.</returns>
        private async Task<TResult> SendAsync<TResult>(HttpRequestMessage request)
        {
            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TResult>(json);
            }
        }
    }
}