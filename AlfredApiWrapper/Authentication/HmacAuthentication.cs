using System;
using System.Security.Cryptography;
using System.Text;

namespace TagShelf.Alfred.ApiWrapper.Authentication
{
    public class HmacAuthentication
    {
        private readonly string _secretKey;
        private readonly string _apiKey;

        public HmacAuthentication(string secretKey, string apiKey)
        {
            _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public string GenerateSignature(string requestUri, string requestMethod, string requestBody = "")
        {
            var requestTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString();
            
            // Encode and format the request URI
            var encodedUri = Uri.EscapeDataString(requestUri).ToLower();
            var requestContentBase64String = string.IsNullOrEmpty(requestBody) ? "" : Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(requestBody)));

            var signatureRawData = $"{_secretKey}{requestMethod}{encodedUri}{requestTimeStamp}{nonce}{requestContentBase64String}";
            var secretByteArray = Convert.FromBase64String(_apiKey);
            byte[] signatureBytes;

            using (var hmac = new HMACSHA256(secretByteArray))
            {
                signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureRawData));
            }

            var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
            return $"amx {_secretKey}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}";
        }
    }
}
