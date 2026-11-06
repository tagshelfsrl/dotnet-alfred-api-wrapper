using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public HttpApiClient() : this(new HttpClientHandler())
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpApiClient class using the supplied message handler.
        /// Intended for testing, where a fake handler supplies canned responses in place of the network.
        /// </summary>
        /// <param name="handler">The message handler that processes outgoing requests.</param>
        internal HttpApiClient(HttpMessageHandler handler)
        {
            _httpClient = new HttpClient(handler);
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
        /// Gets the configured base address, or <c>null</c> if none has been set. For testing.
        /// </summary>
        internal Uri BaseAddress => _httpClient.BaseAddress;

        /// <summary>
        /// Returns the value(s) of a default request header, or <c>null</c> when absent. For testing.
        /// </summary>
        /// <param name="name">The header name.</param>
        internal string GetDefaultHeaderValue(string name)
        {
            return _httpClient.DefaultRequestHeaders.TryGetValues(name, out var values)
                ? string.Join(",", values)
                : null;
        }

        /// <summary>
        /// Signs each outgoing request just before it is sent (used by HMAC authentication, where every request
        /// carries its own signature). The request's URI is already absolute when the signer runs.
        /// </summary>
        internal Func<HttpRequestMessage, Task> RequestSigner { get; set; }

        /// <summary>
        /// Resolves the request's URI against the base address, as HttpClient would, then applies the
        /// <see cref="RequestSigner"/> when one is set.
        /// </summary>
        private async Task PrepareAsync(HttpRequestMessage request)
        {
            if (RequestSigner == null)
            {
                return;
            }
            if (!request.RequestUri.IsAbsoluteUri && _httpClient.BaseAddress != null)
            {
                request.RequestUri = new Uri(_httpClient.BaseAddress, request.RequestUri);
            }
            await RequestSigner(request).ConfigureAwait(false);
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
        /// Sends a PATCH request to the specified Uri as an asynchronous operation and deserializes the JSON response.
        /// This method ensures that an empty content request is still sent with a Content-Type of application/json.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized result.</typeparam>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server, or null if no content is being sent.</param>
        /// <returns>A task that represents the asynchronous operation, containing the deserialized response.</returns>
        public Task<TResult> PatchAsync<TResult>(string uri, HttpContent content = null)
        {
            // HttpMethod.Patch is unavailable on net472/netstandard2.0, so construct it explicitly.
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
            {
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
        /// Sends a DELETE request to the specified Uri as an asynchronous operation and deserializes the JSON response.
        /// An empty response body yields the default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized result.</typeparam>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <returns>A task that represents the asynchronous operation, containing the deserialized response.</returns>
        public Task<TResult> DeleteAsync<TResult>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return SendAsync<TResult>(request);
        }

        /// <summary>
        /// Fetches the response for a file download request without deserializing the content, allowing direct access to the byte stream.
        /// </summary>
        /// <param name="uri">The URI the request is sent to.</param>
        /// <returns>A task that represents the asynchronous operation, containing the raw HTTP response.</returns>
        public async Task<HttpResponseMessage> GetFileAsync(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*")); // Accept any content type
            await PrepareAsync(request).ConfigureAwait(false);
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response;
        }


        /// <summary>
        /// Sends an HTTP request as an asynchronous operation and deserializes the JSON response to a specified type.
        /// Handles non-2xx responses by attempting to deserialize the error response and throwing a detailed exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the deserialized result.</typeparam>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>A task that represents the asynchronous operation, containing the deserialized response.</returns>
        /// <exception cref="ApiException">Thrown when the response indicates an error.</exception>
        private async Task<TResult> SendAsync<TResult>(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                await PrepareAsync(request).ConfigureAwait(false);
                response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<TResult>(json);
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var errorResult = ParseError(errorJson);
                    throw new ApiException(errorResult.Error, errorResult.Message, response.StatusCode, errorJson);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException("Network error", ex.Message, null, null, ex);
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <summary>
        /// Reads an error response body. The API answers errors in several shapes: an object with
        /// <c>error</c>/<c>message</c>, a bare JSON string (for example a 409 from reprocess), or no body
        /// at all (for example most 404s). Every shape yields a result rather than an exception, so the
        /// caller always receives an <see cref="ApiException"/> carrying the status code.
        /// </summary>
        /// <param name="body">The raw response body.</param>
        /// <returns>The parsed error; fields are null when the body does not carry them.</returns>
        internal static ErrorResult ParseError(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return new ErrorResult();
            }

            try
            {
                var token = JToken.Parse(body);
                if (token.Type == JTokenType.Object)
                {
                    return token.ToObject<ErrorResult>() ?? new ErrorResult();
                }
                if (token.Type == JTokenType.String)
                {
                    return new ErrorResult { Message = token.Value<string>() };
                }
            }
            catch (JsonException)
            {
                // Not JSON (for example an HTML error page): fall through and keep the raw text.
            }

            return new ErrorResult { Message = body };
        }
    }
}