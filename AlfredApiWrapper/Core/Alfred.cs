using System;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession;
using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.Job;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Represents the main client for interacting with the Alfred API, supporting various authentication methods.
    /// </summary>
    public class Alfred : IAlfred
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
            InitDomains();
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

        /// <summary>
        /// Initializes the domain-specific properties of the Alfred API wrapper.
        /// </summary>
        private void InitDomains()
        {
            Job = new JobDomain(ApiClient);
            File = new FileDomain(ApiClient);
            Tagshelf = new TagshelfDomain(ApiClient);
            DeferredSession = new DeferredSessionDomain(ApiClient);
        }

        #region IAlfred implementation
        public JobDomain Job { get; private set; }
        public FileDomain File { get; private set; }
        public TagshelfDomain Tagshelf { get; private set; }
        public DeferredSessionDomain DeferredSession { get; private set; }
        #endregion
    }
}
