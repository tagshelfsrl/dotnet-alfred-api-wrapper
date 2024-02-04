using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Results;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job
{
    /// <summary>
    /// Encapsulates job-related operations for the Alfred API.
    /// </summary>
    public class JobDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobDomain"/> class.
        /// </summary>
        /// <param name="apiClient">The HTTP client for API communication.</param>
        public JobDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Creates a new Job in Alfred by finalizing the deferred upload session and initiating document processing.
        /// </summary>
        /// <param name="request">The request details for creating the job.</param>
        /// <returns>The result of the job creation, including the job ID.</returns>
        public async Task<CreateJobResult> CreateAsync(CreateJobRequest request)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var jsonRequest = JsonConvert.SerializeObject(request, Formatting.None, settings);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            return await _apiClient.PostAsync<CreateJobResult>("api/job/create", content).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches detailed information about a specific Job by its unique ID.
        /// </summary>
        /// <param name="jobId">The unique identifier of the Job.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the detailed information about the Job.</returns>
        public async Task<JobDetailResult> GetAsync(Guid jobId)
        {
            return await _apiClient.GetAsync<JobDetailResult>($"api/job/detail/{jobId}").ConfigureAwait(false);
        }
    }
}
