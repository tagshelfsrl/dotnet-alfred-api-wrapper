using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AlfredApiWrapper.Domains.File.Requests;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;

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
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent);

                content.Add(new StringContent(request.SessionId.ToString()), "session_id");

                if (!string.IsNullOrEmpty(request.Metadata))
                {
                    content.Add(new StringContent(request.Metadata, Encoding.UTF8, "application/json"), "metadata");
                }

                return await _apiClient.PostAsync<UploadFileResult>("/api/file/uploadfile", content).ConfigureAwait(false);
            }
        }
    }
}
