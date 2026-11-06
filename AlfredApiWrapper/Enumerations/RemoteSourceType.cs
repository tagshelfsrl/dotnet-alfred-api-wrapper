namespace TagShelf.Alfred.ApiWrapper.Enumerations
{
    /// <summary>
    /// The cloud storage provider of a remote source.
    /// </summary>
    public enum RemoteSourceType
    {
        /// <summary>Amazon S3.</summary>
        Aws = 0,

        /// <summary>DigitalOcean Spaces.</summary>
        DigitalOcean = 1,

        /// <summary>Azure Blob Storage.</summary>
        Azure = 2
    }
}
