using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

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
        /// <returns>The result of the job creation, including the job ID and the correlation ID it was submitted with.</returns>
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
        /// <param name="exclude">
        /// Optional sections to leave out of the response, which makes it smaller and faster for large jobs.
        /// Defaults to <see cref="JobDetailExclude.None"/> (the full detail).
        /// </param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the detailed information about the Job.</returns>
        public async Task<JobDetailResult> GetAsync(Guid jobId, JobDetailExclude exclude = JobDetailExclude.None)
        {
            var uri = $"api/job/detail/{jobId}" + QueryString.Build(QueryString.Param("exclude", BuildExcludeValue(exclude)));
            return await _apiClient.GetAsync<JobDetailResult>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the tenant's jobs, one page at a time, optionally filtered by date window, correlation ID or channel.
        /// </summary>
        /// <param name="request">Filters and paging. Leave null for the first page of the newest jobs.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested page of jobs and the total count.</returns>
        public async Task<PagedResult<JobSummaryResult>> GetAllAsync(JobListRequest request = null)
        {
            request = request ?? new JobListRequest();
            var uri = "api/job/all" + QueryString.Build(
                QueryString.Param("currentPage", request.Page.ToString(CultureInfo.InvariantCulture)),
                QueryString.Param("pageSize", request.PageSize.ToString(CultureInfo.InvariantCulture)),
                QueryString.Param("sortDirection", request.SortDirection == SortDirection.Ascending ? "ASC" : "DESC"),
                QueryString.Param("startDate", FormatDate(request.StartDate)),
                QueryString.Param("endDate", FormatDate(request.EndDate)),
                QueryString.Param("correlation_id", request.CorrelationId),
                QueryString.Param("channel", request.Channel));
            return await _apiClient.GetAsync<PagedResult<JobSummaryResult>>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Restarts a job, running its files through processing again.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <param name="redownloadRemoteFiles">
        /// When true, the job's remote inputs (URLs and remote storage files) are downloaded again instead of
        /// reusing the copies Alfred already holds. The restart is refused (an <see cref="ApiException"/> with
        /// status 409) when any of them can no longer be reached. Defaults to false.
        /// </param>
        /// <returns>A task that represents the asynchronous operation. The task result says whether the job was restarted.</returns>
        public async Task<JobRestartResult> RestartAsync(Guid jobId, bool redownloadRemoteFiles = false)
        {
            var uri = "api/job/restart" + QueryString.Build(
                QueryString.Param("jobId", jobId.ToString()),
                QueryString.Flag("redownloadRemoteFiles", redownloadRemoteFiles));
            return await _apiClient.PostAsync<JobRestartResult>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels a job.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>A task that represents the asynchronous operation. The task result says whether the job was canceled.</returns>
        public async Task<JobCancelResult> CancelAsync(Guid jobId)
        {
            var uri = "api/job/cancel" + QueryString.Build(QueryString.Param("jobId", jobId.ToString()));
            return await _apiClient.PostAsync<JobCancelResult>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks whether a job's remote input sources can still be reached, for example before restarting it
        /// with <c>redownloadRemoteFiles</c>.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>A task that represents the asynchronous operation. The task result lists any sources that could not be reached.</returns>
        public async Task<JobSourcesCheckResult> CheckSourcesAsync(Guid jobId)
        {
            var uri = "api/job/check-sources" + QueryString.Build(QueryString.Param("jobId", jobId.ToString()));
            return await _apiClient.PostAsync<JobSourcesCheckResult>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Reprocesses or re-extracts a chosen set of a job's files in one call. Each file is checked on its
        /// own: files that are not in the job, or not eligible (for example re-extracting a file that was never
        /// classified), are skipped and reported rather than failing the whole request.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <param name="request">The files and the operation to run.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains one outcome per requested file.</returns>
        public async Task<BulkFileOperationResult> BulkFileOperationAsync(Guid jobId, BulkFileOperationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var uri = "api/job/bulk-file-operation" + QueryString.Build(QueryString.Param("jobId", jobId.ToString()));
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            return await _apiClient.PostAsync<BulkFileOperationResult>(uri, content).ConfigureAwait(false);
        }

        /// <summary>Maps <see cref="JobDetailExclude"/> flags to the comma-separated <c>exclude</c> query value, or null when none.</summary>
        internal static string BuildExcludeValue(JobDetailExclude exclude)
        {
            var parts = new List<string>();
            if ((exclude & JobDetailExclude.Files) != 0)
            {
                parts.Add("files");
            }
            if ((exclude & JobDetailExclude.Metadata) != 0)
            {
                parts.Add("metadata");
            }
            if ((exclude & JobDetailExclude.FileUrls) != 0)
            {
                parts.Add("fileurls");
            }
            if ((exclude & JobDetailExclude.ErrorMessages) != 0)
            {
                parts.Add("errormessages");
            }
            return parts.Count == 0 ? null : string.Join(",", parts);
        }

        // The job list's date window is date-only (UTC day), sent as yyyy-MM-dd.
        private static string FormatDate(DateTime? date)
        {
            return date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
