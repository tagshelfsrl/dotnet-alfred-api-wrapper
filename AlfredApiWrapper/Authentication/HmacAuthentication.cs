using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TagShelf.Alfred.ApiWrapper.Authentication
{
    /// <summary>
    /// Signs requests with Alfred's HMAC scheme (<c>Authorization: amx ...</c>).
    /// <para>
    /// Every request carries its own signature: an HMAC-SHA256, keyed with the API key, over the secret key,
    /// the HTTP method, the URL-encoded lower-cased request URL, a Unix timestamp, a one-time nonce and the
    /// Base64 MD5 of the body. The server rejects a signature older than five minutes and any nonce it has
    /// already seen.
    /// </para>
    /// </summary>
    public class HmacAuthentication
    {
        /// <summary>The Authorization scheme Alfred uses for HMAC.</summary>
        internal const string Scheme = "amx";

        private readonly string _secretKey;
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="HmacAuthentication"/> class.
        /// </summary>
        /// <param name="secretKey">The user's secret key. It identifies the user and is sent with every request.</param>
        /// <param name="apiKey">The user's API key (Base64). It keys the signature and is never sent.</param>
        /// <exception cref="ArgumentException">The API key is not Base64.</exception>
        public HmacAuthentication(string secretKey, string apiKey)
        {
            _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            try
            {
                Convert.FromBase64String(apiKey);
            }
            catch (FormatException)
            {
                // Caught here, once, instead of on the first request the client sends.
                throw new ArgumentException("The HMAC API key must be the Base64 key Alfred issued.", nameof(apiKey));
            }
        }

        /// <summary>
        /// Generates the value of the Authorization header for one request.
        /// </summary>
        /// <param name="requestUri">The absolute URL of the request, including its query string.</param>
        /// <param name="requestMethod">The HTTP method (for example "GET").</param>
        /// <param name="requestBody">The request body, or empty for none.</param>
        /// <returns>The full header value, <c>amx {secretKey}:{signature}:{nonce}:{timestamp}</c>.</returns>
        public string GenerateSignature(string requestUri, string requestMethod, string requestBody = "")
        {
            var body = string.IsNullOrEmpty(requestBody) ? new byte[0] : Encoding.UTF8.GetBytes(requestBody);
            return Scheme + " " + CreateParameter(requestUri, requestMethod, body, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        /// <summary>
        /// Signs a request in place by setting its Authorization header. The body is read (and buffered) to hash it.
        /// </summary>
        /// <param name="request">The request, whose <see cref="HttpRequestMessage.RequestUri"/> must be absolute.</param>
        /// <returns>A task that completes when the header is set.</returns>
        internal async Task SignAsync(HttpRequestMessage request)
        {
            var body = request.Content == null
                ? new byte[0]
                : await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var parameter = CreateParameter(
                request.RequestUri.AbsoluteUri,
                request.Method.Method,
                body,
                Guid.NewGuid().ToString(),
                DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            request.Headers.Authorization = new AuthenticationHeaderValue(Scheme, parameter);
        }

        /// <summary>
        /// Builds the Authorization parameter <c>{secretKey}:{signature}:{nonce}:{timestamp}</c>.
        /// </summary>
        internal string CreateParameter(string absoluteUri, string method, byte[] body, string nonce, long timestamp)
        {
            var requestTimeStamp = timestamp.ToString(CultureInfo.InvariantCulture);
            var bodyHash = string.Empty;
            if (body != null && body.Length > 0)
            {
                using (var md5 = MD5.Create())
                {
                    bodyHash = Convert.ToBase64String(md5.ComputeHash(body));
                }
            }

            var signatureRawData = _secretKey + method.ToUpperInvariant() + EncodeUri(absoluteUri) + requestTimeStamp + nonce + bodyHash;
            byte[] signatureBytes;
            using (var hmac = new HMACSHA256(Convert.FromBase64String(_apiKey)))
            {
                signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureRawData));
            }

            return $"{_secretKey}:{Convert.ToBase64String(signatureBytes)}:{nonce}:{requestTimeStamp}";
        }

        /// <summary>
        /// Encodes the request URL the way the server does before comparing signatures: lower-cased, then
        /// form-URL-encoded (unreserved characters <c>-_.!*()</c> kept, every other symbol percent-encoded with
        /// lower-case hex digits).
        /// </summary>
        internal static string EncodeUri(string absoluteUri)
        {
            // The input is lower-cased first, so the only upper-case characters left in the output are the
            // hex digits of the escapes WebUtility writes; the server writes those in lower case.
            return WebUtility.UrlEncode(absoluteUri.ToLowerInvariant()).ToLowerInvariant();
        }
    }
}
