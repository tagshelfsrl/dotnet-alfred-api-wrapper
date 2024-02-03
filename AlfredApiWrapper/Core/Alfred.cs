using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.Job;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Represents the primary entry point for interacting with the Alfred API.
    /// </summary>
    public class Alfred : IAlfred
    {
        private HttpApiClient _client;

        /// <summary>
        /// Initializes a new instance of the Alfred class with API key authentication,
        /// allowing specification of the environment, defaulting to Production.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="environment">The environment to connect to, defaulting to Production.</param>
        public Alfred(string apiKey, EnvironmentType environment = EnvironmentType.Production)
        {
            // Assuming HttpApiClient's constructor can handle environment-specific base URLs
            _client = new HttpApiClient(apiKey, environment);
            InitializeDomains();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alfred"/> class with OAuth or HMAC authentication.
        /// </summary>
        /// <param name="keyOrUsername">The API key or username for authentication.</param>
        /// <param name="secretOrPassword">The secret or password for authentication.</param>
        /// <param name="authMethod">The authentication method.</param>
        /// <param name="environment">The environment to connect to.</param>
        public Alfred(string keyOrUsername, string secretOrPassword, AuthenticationMethod authMethod, EnvironmentType environment)
        {
            // Implementation details here
            InitializeDomains();
        }

        /// <summary>
        /// Provides access to job-related operations.
        /// </summary>
        public JobDomain Job { get; private set; }

        /// <summary>
        /// Provides access to file-related operations.
        /// </summary>
        public FileDomain File { get; private set; }

        /// <summary>
        /// Initializes domain-specific functionalities.
        /// </summary>
        private void InitializeDomains()
        {
            // Assuming JobDomain and FileDomain are correctly implemented elsewhere
            Job = new JobDomain(_client);
            File = new FileDomain(_client);
        }
    }
}
