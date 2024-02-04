using System;

namespace TagShelf.Alfred.ApiWrapper.Core
{

    /// <summary>
    /// Represents errors that occur during API request execution.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets the error code returned by the API.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets the error message returned by the API.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the HTTP status code of the response, if available.
        /// </summary>
        public System.Net.HttpStatusCode? StatusCode { get; }

        /// <summary>
        /// Gets the raw content of the error response.
        /// </summary>
        public string ResponseContent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class with specified error details.
        /// </summary>
        /// <param name="error">The error code returned by the API.</param>
        /// <param name="errorMessage">The error message returned by the API.</param>
        /// <param name="statusCode">The HTTP status code of the response, if available.</param>
        /// <param name="responseContent">The raw content of the error response.</param>
        /// <param name="innerException">The inner exception, if any.</param>
        public ApiException(string error, string errorMessage, System.Net.HttpStatusCode? statusCode, string responseContent, Exception innerException = null)
            : base($"API error: {error} - {errorMessage}", innerException)
        {
            Error = error;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}