using System;
using System.Configuration;

namespace protectTreesV2.Base
{
    public static class VirtualPathHelper
    {
        public static string ApplyVirtualName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            string normalizedPath = NormalizePath(path);
            string virtualName = NormalizeVirtualName(ConfigurationManager.AppSettings["virturlName"]);

            if (string.IsNullOrWhiteSpace(virtualName))
            {
                return normalizedPath;
            }

            if (normalizedPath.StartsWith(virtualName + "/", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalizedPath, virtualName, StringComparison.OrdinalIgnoreCase))
            {
                return normalizedPath;
            }

            return virtualName + normalizedPath;
        }

        private static string NormalizeVirtualName(string virtualName)
        {
            if (string.IsNullOrWhiteSpace(virtualName))
            {
                return string.Empty;
            }

            string trimmed = virtualName.Trim();
            if (trimmed == "/")
            {
                return string.Empty;
            }

            trimmed = trimmed.Trim('/');
            return string.IsNullOrWhiteSpace(trimmed) ? string.Empty : "/" + trimmed;
        }

        private static string NormalizePath(string path)
        {
            string trimmed = path.Trim();

            if (trimmed.StartsWith("~/", StringComparison.Ordinal))
            {
                trimmed = trimmed.Substring(1);
            }

            if (!trimmed.StartsWith("/", StringComparison.Ordinal))
            {
                trimmed = "/" + trimmed;
            }

            return trimmed;
        }
    }
}
