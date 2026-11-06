using System;

namespace TagShelf.Alfred.ApiWrapper.Domains.Search.Requests
{
    /// <summary>
    /// A search of document content within the tags the caller can see.
    /// </summary>
    public class SearchRequest
    {
        /// <summary>The text to match against document content. Required.</summary>
        public string Query { get; set; }

        /// <summary>Only search files of this tag. Leave null (and <see cref="TagName"/> null) to search every accessible tag.</summary>
        public Guid? TagId { get; set; }

        /// <summary>Only search files of the tag with this name (case-insensitive). An alternative to <see cref="TagId"/>.</summary>
        public string TagName { get; set; }

        /// <summary>The 1-based page number. Defaults to 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>The number of results per page. Defaults to 50; at most 100 is recommended.</summary>
        public int PageSize { get; set; } = 50;

        /// <summary>When true, approximate matches are included. Defaults to false.</summary>
        public bool Fuzzy { get; set; }
    }
}
