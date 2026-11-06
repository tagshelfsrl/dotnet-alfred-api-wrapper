using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Results;
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Results;

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

        /// <summary>
        /// Lists the accounts (tenants) the caller has access to. An API key belongs to one account; a user
        /// signed in with OAuth or HMAC may have access to several.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the accounts.</returns>
        public async Task<List<CompanyResult>> GetCompaniesAsync()
        {
            return await _apiClient.GetAsync<List<CompanyResult>>("api/account/companies").ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the tags the caller can see in an account.
        /// </summary>
        /// <param name="companyId">
        /// The account to list tags for; one of <see cref="GetCompaniesAsync"/>. Leave null for the caller's own account.
        /// </param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the tags' IDs, names and descriptions.</returns>
        public async Task<List<TagResult>> GetTagsAsync(Guid? companyId = null)
        {
            var uri = companyId.HasValue ? $"api/account/tags/{companyId.Value}" : "api/account/tags";
            return await _apiClient.GetAsync<List<TagResult>>(uri).ConfigureAwait(false);
        }
    }
}
