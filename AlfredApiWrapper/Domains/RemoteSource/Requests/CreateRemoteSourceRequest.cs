using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Domains.RemoteSource.Requests
{
    /// <summary>
    /// A cloud storage location Alfred can read files from. Once created, uploads can reference it by name
    /// (<c>UploadRequest.Source</c>) together with a container and file name.
    /// </summary>
    public class CreateRemoteSourceRequest
    {
        /// <summary>The name uploads use to refer to the source. Required.</summary>
        [JsonProperty("source_name")]
        public string SourceName { get; set; }

        /// <summary>The cloud storage provider. Required.</summary>
        [JsonIgnore]
        public RemoteSourceType Type { get; set; }

        /// <summary>The wire value of <see cref="Type"/>.</summary>
        [JsonProperty("type")]
        internal string TypeValue
        {
            get
            {
                switch (Type)
                {
                    case RemoteSourceType.DigitalOcean:
                        return "digital_ocean";
                    case RemoteSourceType.Azure:
                        return "azure";
                    default:
                        return "aws";
                }
            }
        }

        /// <summary>The storage account or bucket name.</summary>
        [JsonProperty("account_name", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountName { get; set; }

        /// <summary>The access key (AWS and DigitalOcean).</summary>
        [JsonProperty("access_key", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessKey { get; set; }

        /// <summary>The secret key used to read from the storage. Alfred keeps it in the tenant's secret vault.</summary>
        [JsonProperty("secret_key", NullValueHandling = NullValueHandling.Ignore)]
        public string SecretKey { get; set; }

        /// <summary>The region the storage is hosted in.</summary>
        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public string Region { get; set; }

        /// <summary>
        /// Returns a string that represents the current object. The credentials are left out.
        /// </summary>
        /// <returns>A string that contains the source name and type.</returns>
        public override string ToString()
        {
            return $"{SourceName} ({Type})";
        }
    }
}
