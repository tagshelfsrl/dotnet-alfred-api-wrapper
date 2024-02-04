using System;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Represents the main client for interacting with the Alfred API, supporting various authentication methods.
    /// </summary>
    public class Alfred
    {
        /// <summary>
        /// Gets the HttpApiClient used for all HTTP interactions.
        /// </summary>
        internal HttpApiClient ApiClient { get; private set; }

        private EnvironmentType _environment;

        /// <summary>
        /// Initializes a new instance of the Alfred class for a specific environment.
        /// This constructor is internal to ensure instances are created through the AlfredFactory for proper initialization.
        /// </summary>
        /// <param name="environment">The environment to which the Alfred instance will connect.</param>
        internal Alfred(EnvironmentType environment)
        {
            ApiClient = new HttpApiClient();
            _environment = environment;
            SetBaseAddress(environment);
        }

        /// <summary>
        /// Configures the base address of the API client based on the specified environment.
        /// </summary>
        /// <param name="environment">The target environment.</param>
        private void SetBaseAddress(EnvironmentType environment)
        {
            string baseUrl;
            if (environment == EnvironmentType.Production)
            {
                baseUrl = "https://app.tagshelf.com";
            }
            else if (environment == EnvironmentType.Staging)
            {
                baseUrl = "https://staging.tagshelf.com";
            }
            else
            {
                throw new ArgumentException("Unsupported environment type", nameof(environment));
            }
            ApiClient.SetBaseAddress(baseUrl);
        }        
    }
}
