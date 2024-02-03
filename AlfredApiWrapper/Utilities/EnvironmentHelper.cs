using System;

namespace TagShelf.Alfred.ApiWrapper.Utilities
{
    public static class EnvironmentHelper
    {
        public static string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
