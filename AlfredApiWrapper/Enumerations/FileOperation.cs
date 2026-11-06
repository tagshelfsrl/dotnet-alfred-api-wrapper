namespace TagShelf.Alfred.ApiWrapper.Enumerations
{
    /// <summary>
    /// An operation that runs a file through processing again.
    /// </summary>
    public enum FileOperation
    {
        /// <summary>Run the full pipeline again: transformation, classification and extraction.</summary>
        Reprocess = 0,

        /// <summary>Run extraction again with the file's current tag. The file must already be classified.</summary>
        Reextract = 1
    }
}
