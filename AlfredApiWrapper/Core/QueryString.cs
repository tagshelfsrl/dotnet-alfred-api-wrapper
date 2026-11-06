using System;
using System.Collections.Generic;

namespace TagShelf.Alfred.ApiWrapper.Core
{
    /// <summary>
    /// Builds URL query strings for API requests.
    /// </summary>
    internal static class QueryString
    {
        /// <summary>A named query parameter. A null or empty value leaves the parameter out.</summary>
        internal static KeyValuePair<string, string> Param(string name, string value)
        {
            return new KeyValuePair<string, string>(name, value);
        }

        /// <summary>A boolean flag that is only sent when true; false is the server default, so it is left out.</summary>
        internal static KeyValuePair<string, string> Flag(string name, bool value)
        {
            return new KeyValuePair<string, string>(name, value ? "true" : null);
        }

        /// <summary>
        /// Builds a query string ("?a=b&amp;c=d") from the supplied parameters, skipping any with a
        /// null/empty value and URL-encoding the rest. Returns an empty string when nothing is set.
        /// </summary>
        internal static string Build(params KeyValuePair<string, string>[] parameters)
        {
            var parts = new List<string>();
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter.Value))
                {
                    continue;
                }
                parts.Add(Uri.EscapeDataString(parameter.Key) + "=" + Uri.EscapeDataString(parameter.Value));
            }
            return parts.Count == 0 ? string.Empty : "?" + string.Join("&", parts);
        }
    }
}
