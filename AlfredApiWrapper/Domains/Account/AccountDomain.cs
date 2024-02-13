using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains
{
    public class AccountDomain
    {
        private readonly HttpApiClient _apiClient;

        public AccountDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Sets up a webhook for the account with the specified URL and returns the webhook's ID.
        /// </summary>
        /// <param name="request">The request containing the URL of the webhook endpoint.</param>
        /// <returns>A task representing the asynchronous operation, containing the webhook setup response.</returns>
        public async Task<WebhookSetupResult> SetupWebhookAsync(WebhookSetupRequest request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync<WebhookSetupResult>("/api/account/webhook", content);
            return response;
        }
    }
}
