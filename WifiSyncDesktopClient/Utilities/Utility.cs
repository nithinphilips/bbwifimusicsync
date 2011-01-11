using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;

namespace QDownloader.Utilities
{
    public sealed class Utility
    {

        // ShortenPath(...) methods from http://www.codinghorror.com/blog/archives/000650.html
        
        // The API function will give you an output of exact length.
        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathCompactPathEx([Out]StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        /// <summary>
        /// Makes a path short using PathCompactPathEx WIN32 API call.
        /// </summary>
        public static string ShortenPath(string path, int maxLength)
        {
            StringBuilder sb = new StringBuilder();
            PathCompactPathEx(sb, path, maxLength, 0);
            return sb.ToString();
        }

        /// <summary>
        /// Makes a path short using regex. File Name and root name is preserved.
        /// </summary>
        public static string ShortenPath(string path)
        {
            const string pattern = @"^(\w+:|\\)(\\[^\\]+\\[^\\]+\\).*(\\[^\\]+\\[^\\]+)$";
            const string replacement = "$1$2...$3";
            if (Regex.IsMatch(path, pattern, RegexOptions.Singleline))
            {
                return Regex.Replace(path, pattern, replacement);
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// Makes a url short using regex. Supports http, https, ftp, and file protocol prefixes.
        /// </summary>
        public static string ShortenUrl(string path)
        {
            // const string pattern = @"^(http|https|ftp|file)(://[^/]+/).*(/[^\n\r\\x00?].*)$";
            const string pattern = @"^(http|https|ftp|file)(://[^/]+/).*(/[^$\?].*)$";
            const string replacement = "$1$2...$3";
            if (Regex.IsMatch(path, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase))
            {
                return Regex.Replace(path, pattern, replacement);
            }
            else
            {
                return path;
            }
        }

        public static void LocateOnDisk(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
        }

    }
}
