using Newtonsoft.Json;

namespace TagShelf.Alfred.ApiWrapper.Domains.File.Requests
{
    /// <summary>
    /// How a shareable link to a file behaves. Set either <see cref="ExpiresInSeconds"/> or
    /// <see cref="DoesNotExpire"/>.
    /// </summary>
    public class ShareFileRequest
    {
        /// <summary>How long the link works, in seconds (at least 60). Ignored when <see cref="DoesNotExpire"/> is true.</summary>
        [JsonProperty("length", NullValueHandling = NullValueHandling.Ignore)]
        public int? ExpiresInSeconds { get; set; }

        /// <summary>When true, the link never expires. Defaults to false.</summary>
        [JsonProperty("does_not_expire")]
        public bool DoesNotExpire { get; set; }

        /// <summary>
        /// When true (the default), the link always serves the file's latest version. When false, it serves the
        /// version current when the link was created.
        /// </summary>
        [JsonProperty("always_use_last_version")]
        public bool AlwaysUseLastVersion { get; set; } = true;

        /// <summary>
        /// When true (the default), a short link is returned. If shortening fails, the full link is returned instead.
        /// </summary>
        [JsonProperty("short_url")]
        public bool ShortUrl { get; set; } = true;
    }
}
