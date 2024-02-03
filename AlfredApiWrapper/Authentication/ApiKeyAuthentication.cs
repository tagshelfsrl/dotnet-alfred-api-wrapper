using System.Net.Http;

namespace TagShelf.Alfred.ApiWrapper.Authentication
{
    public class ApiKeyAuthentication
    {
        public ApiKeyAuthentication(HttpClient httpClient, string apiKey)
        {
            httpClient.DefaultRequestHeaders.Add("X-TagshelfAPI-Key", apiKey);
        }
    }
}
