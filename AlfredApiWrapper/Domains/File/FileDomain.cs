using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AlfredApiWrapper.Domains.File.Requests;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.File.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;
using TagShelf.Alfred.ApiWrapper.Utilities;

namespace TagShelf.Alfred.ApiWrapper.Domains.File
{
    /// <summary>
    /// Encapsulates file-related operations for the Alfred API.
    /// </summary>
    public class FileDomain
    {
        private readonly HttpApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDomain"/> class.
        /// </summary>
        /// <param name="client">The HTTP client for API communication.</param>
        public FileDomain(HttpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Fetches detailed information about a specific file by its ID.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the detailed information about the file.</returns>
        public async Task<FileDetailResult> GetAsync(Guid fileId)
        {
            return await _apiClient.GetAsync<FileDetailResult>($"api/file/detail/{fileId}").ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads a file by its ID and constructs a FileDownloadResult.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file to download.</param>
        /// <returns>A FileDownloadResult containing the file's content type, name, and contents.</returns>
        public async Task<FileDownloadResult> DownloadAsync(Guid fileId)
        {
            var response = await _apiClient.GetFileAsync($"api/file/download/{fileId}");

            var result = new FileDownloadResult
            {
                ContentType = response.Content.Headers.ContentType?.MediaType,
                FileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"'),
                FileContents = await response.Content.ReadAsStreamAsync()
            };

            return result;
        }

        /// <summary>
        /// Uploads remote files to Alfred for processing.
        /// </summary>
        /// <param name="request">The details of the files to upload.</param>
        /// <returns>The job ID associated with the uploaded files.</returns>
        public async Task<FileUploadResult> UploadAsync(UploadRequest request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync<FileUploadResult>("api/file/upload", content).ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Uploads a single remote file to the system's pipeline using a request object.
        /// </summary>
        /// <param name="request">The details of the file and metadata to upload.</param>
        /// <returns>The result of the file upload operation, including the file ID.</returns>
        public async Task<UploadFileResult> UploadFileAsync(UploadFileRequest request)
        {
            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new StreamContent(request.FileStream);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"file\"",
                    FileName = "\"" + request.FileName + "\""
                };

                // Use the caller-provided content type when available; otherwise
                // infer it from the file name extension so the server receives a
                // meaningful Content-Type instead of a generic octet-stream.
                var contentType = !string.IsNullOrEmpty(request.ContentType)
                    ? request.ContentType
                    : MimeTypeHelper.GetMimeType(request.FileName);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(fileContent);

                content.Add(new StringContent(request.SessionId.ToString()), "session_id");

                if (!string.IsNullOrEmpty(request.Metadata))
                {
                    content.Add(new StringContent(request.Metadata, Encoding.UTF8, "application/json"), "metadata");
                }

                return await _apiClient.PostAsync<UploadFileResult>("/api/file/uploadfile", content).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Lists the tenant's files, one page at a time.
        /// </summary>
        /// <param name="page">The 1-based page number. Defaults to 1.</param>
        /// <param name="pageSize">The number of files per page. The server caps this at 200. Defaults to 25.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested page of files and the total count.</returns>
        public async Task<PagedResult<FileDetailResult>> GetAllAsync(int page = 1, int pageSize = 25)
        {
            var uri = "api/file/all" + QueryString.Build(
                QueryString.Param("currentPage", page.ToString(CultureInfo.InvariantCulture)),
                QueryString.Param("pageSize", pageSize.ToString(CultureInfo.InvariantCulture)));
            return await _apiClient.GetAsync<PagedResult<FileDetailResult>>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the events Alfred raised for a file and whether each was delivered to the event broker and the webhook.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the file's events.</returns>
        public async Task<List<FileEventResult>> GetEventsAsync(Guid fileId)
        {
            return await _apiClient.GetAsync<List<FileEventResult>>($"api/file/events/{fileId}").ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the versions of a file's extracted text.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the file's versions.</returns>
        public async Task<List<FileVersionResult>> GetVersionsAsync(Guid fileId)
        {
            return await _apiClient.GetAsync<List<FileVersionResult>>($"api/file/versions/{fileId}").ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a link that lets anyone download the file without credentials. The file must have finished processing.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="request">How long the link works and which version it serves.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the link.</returns>
        public async Task<ShareFileResult> ShareAsync(Guid fileId, ShareFileRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (!request.DoesNotExpire && (request.ExpiresInSeconds == null || request.ExpiresInSeconds < 60))
            {
                throw new ArgumentException(
                    "Set ExpiresInSeconds to at least 60, or set DoesNotExpire to true.", nameof(request));
            }

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            return await _apiClient.PostAsync<ShareFileResult>($"api/file/share/{fileId}", content).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs a file through the full pipeline again: transformation, classification and extraction. Use it when
        /// the file was classified with the wrong tag or its transformation failed.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="skipCache">When true, cached intermediate results are not reused. Defaults to false.</param>
        /// <param name="requestReview">
        /// When true, the result is held for human review whatever the tag's validation settings say. Defaults to false.
        /// </param>
        /// <returns>A task that represents the asynchronous operation. The task result confirms the file was queued.</returns>
        public async Task<FileOperationResult> ReprocessAsync(Guid fileId, bool skipCache = false, bool requestReview = false)
        {
            var uri = $"api/file/reprocess/{fileId}" + QueryString.Build(
                QueryString.Flag("skipCache", skipCache),
                QueryString.Flag("requestReview", requestReview));
            return await _apiClient.PostAsync<FileOperationResult>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs extraction again on a file with its current tag. Use it when the extracted data is wrong or the
        /// tag's data points changed. The file must already be classified.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="requestReview">
        /// When true, the result is held for human review whatever the tag's validation settings say. Defaults to false.
        /// </param>
        /// <returns>A task that represents the asynchronous operation. The task result confirms the file was queued.</returns>
        public async Task<FileOperationResult> ReextractAsync(Guid fileId, bool requestReview = false)
        {
            var uri = $"api/file/reextract/{fileId}" + QueryString.Build(QueryString.Flag("requestReview", requestReview));
            return await _apiClient.PostAsync<FileOperationResult>(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Gives a file a different tag (document type) and sends it to extraction under the new one, as
        /// "Change type" does in the console. The file and the tag must belong to the caller's account, and a
        /// file someone else is reviewing is refused with 409. Requires Alfred 1.128.4 or later.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="tagId">The unique identifier of the new tag. <see cref="Tag.TagDomain.GetAllAsync"/> lists them.</param>
        /// <returns>A task that represents the asynchronous operation. The task result confirms the change.</returns>
        public async Task<ChangeFileTagResult> ChangeTagAsync(Guid fileId, Guid tagId)
        {
            if (tagId == Guid.Empty)
            {
                throw new ArgumentException("A tag ID is required.", nameof(tagId));
            }

            var body = JsonConvert.SerializeObject(new Dictionary<string, Guid> { { "tag_id", tagId } });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            return await _apiClient.PostAsync<ChangeFileTagResult>($"api/file/{fileId}/tag", content).ConfigureAwait(false);
        }
    }
}
