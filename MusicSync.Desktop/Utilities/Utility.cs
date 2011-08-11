/**********************************************************************
 * WifiMusicSync
 * Copyright (C) 2011 Nithin Philips <nithin@nithinphilips.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 **********************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace WifiSyncDesktop.Utilities
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
            if (string.IsNullOrWhiteSpace(path)) return path;

            StringBuilder sb = new StringBuilder();
            PathCompactPathEx(sb, path, maxLength, 0);
            return sb.ToString();
        }

        /// <summary>
        /// Makes a path short using regex. File Name and root name is preserved.
        /// </summary>
        public static string ShortenPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;

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
            if (string.IsNullOrWhiteSpace(path)) return path;

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

        /// <summary>
        /// Get a list of all file in path
        /// </summary>
        /// <param name="path">The path to search in.</param>
        /// <param name="options">Any options for searching.</param>
        /// <param name="filters">A set of patterns to search for.</param>
        /// <returns>A HashSet of the results that can be used for various Set operations.</returns>
        public static HashSet<string> GetFiles(string path, SearchOption options, params string[] filters)
        {
            HashSet<string> files = new HashSet<string>();

            if (!Directory.Exists(path)) return files;

            try
            {
                foreach (string file in filters.SelectMany(filter => Directory.GetFiles(path, filter, options)))
                {
                    files.Add(file);
                }
            }catch
            {
                // we're just returning whatever we have
                return files;
            }

            return files;
        }
    }
}
