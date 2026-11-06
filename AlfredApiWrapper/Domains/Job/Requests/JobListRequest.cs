using System;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.Domains.Job.Requests
{
    /// <summary>
    /// Filters and paging for listing the tenant's jobs.
    /// </summary>
    public class JobListRequest
    {
        /// <summary>The 1-based page number. Defaults to 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>The number of jobs per page. The server accepts 10 to 100. Defaults to 10.</summary>
        public int PageSize { get; set; } = 10;

        /// <summary>The order of the jobs by creation date. Defaults to newest first.</summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Descending;

        /// <summary>
        /// The first day (UTC) of the date window. Only the date part is used. When set alone, the window
        /// runs from that day through today.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>The last day (UTC) of the date window, inclusive. Only the date part is used. Requires <see cref="StartDate"/>.</summary>
        public DateTime? EndDate { get; set; }

        /// <summary>Only jobs submitted with exactly this correlation ID (case-sensitive).</summary>
        public string CorrelationId { get; set; }

        /// <summary>Only jobs submitted through this channel (for example "api").</summary>
        public string Channel { get; set; }
    }
}
