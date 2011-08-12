/* ************************************************************************
 *  Q Downloader
 *  Copyleft (C) 2005 Nithin Philips
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation,Inc.,59 Temple Place - Suite 330,Boston,MA 02111-1307, USA.
 *
 *  Author            :  Nithin Philips <nithin@nithinphilips.com>
 *  Report Bugs To    :  <bugs-qd@nithinphilips.com>
 *  Original FileName :  Common.cs
 *  Created           :  Sat Feb 12 2005
 *  Description       :  
 *  Description       :  
 * ************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace LibQdownloader.Utilities
{
    public static class Common
    {
        /// <summary>
        /// Converts a size in bytes to human readable form (mb,gb,tb etc.)
        /// </summary>
        public static string ToReadableSize(int sizeinbytes)
        {
            return ToReadableSize((double)sizeinbytes);
        }

        /// <summary>
        /// Converts a size in bytes to human readable form (mb,gb,tb etc.)
        /// </summary>
        public static string ToReadableSize(long sizeinbytes)
        {
            return ToReadableSize((double)sizeinbytes);
        }

        public static string ToReadableSize(double sizeinbytes)
        {
            return ToReadableSize(sizeinbytes, 2);
        }

        /// <summary>
        /// Converts a size in bytes to human readable form (mb,gb,tb etc.)
        /// </summary>
        public static string ToReadableSize(double sizeinbytes, int precision)
        {
            if (sizeinbytes <= 0)
            {
                return "?";
            }
            else if (sizeinbytes < 1024)
            {
                return sizeinbytes + " bytes";
            }
            else if (sizeinbytes < 1048576)
            {
                return Math.Round((sizeinbytes / 1024d), precision) + " KB";
            }
            else if (sizeinbytes < 1073741824)
            {
                return Math.Round((sizeinbytes / 1048576d), precision) + " MB";
            }
            else if (sizeinbytes < 1099511627776)
            {
                return Math.Round((sizeinbytes / 1073741824d), precision) + " GB";
            }
            else if (sizeinbytes < 1125899906842624)
            {
                return Math.Round((sizeinbytes / 1099511627776d), precision) + " TB";
            }
            else
            {
                return sizeinbytes + " bytes!";
            }
        }

        /// <summary>
        /// Returns a unique directory name. If the specified path already exists a new name with a numeric sequence is returned.
        /// </summary>
        public static string GetUniqueDirectoryName(string name)
        {
            string currentName = name;
            int count = 1;
            while (true)
            {
                if (!Directory.Exists(currentName)) return currentName;
                currentName = name + " (" + count.ToString() + ")";
                count++;
            }
        }

        public static string GetFilenameFromUri(Uri uri)
        {
            Mono.IO.Path monoPath = Mono.IO.Path.FromPath(uri.AbsoluteUri);
            string safeName = MakeFileNameSafe(monoPath.GetFileName(uri.AbsoluteUri));
            return safeName;
        }

        public static string MakeFileNameSafe(string name)
        {
            return ReplaceChars(Path.GetInvalidFileNameChars(), "_", name);
        }

        public static string RemoveChars(char[] charsToRemove, string name)
        {
            return ReplaceChars(charsToRemove, "", name);
        }

        public static string ReplaceChars(char[] charsToRemove, string replacement, string name)
        {
            StringBuilder sb = new StringBuilder(name.Length);
            for (int i = 0; i < name.Length; i++)
            {
                if (Array.IndexOf<char>(charsToRemove, name[i]) >= 0)
                {
                    // char is illegal
                    sb.Append(replacement);
                    continue;
                }
                sb.Append(name[i]);
            }
            return sb.ToString();
        }

        public static bool HasInvalidPathChars(string path)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            for (int i = 0; i < path.Length; i++)
            {
                if (Array.IndexOf<char>(invalidPathChars, path[i]) >= 0)
                {
                    // char is illegal
                    return true;
                }
            }
            return false;
        }

        public static string RemoveQuotes(string word)
        {
            return word.Substring(1, word.Length - 2);
        }

        public static void Serialize<T>(T instance, string fileName) where T : class
        {
            string tempFile = Path.GetTempFileName();
            using (System.IO.FileStream fs = new System.IO.FileStream(tempFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                xs.Serialize(fs, instance);
                fs.Flush();
            }
            File.Copy(tempFile, fileName, true);
            File.Delete(tempFile);
        }

        public static T DeSerialize<T>(string fileName) where T : class
        {
            object up;
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                up = xs.Deserialize(fs);
            }
            return up as T;
        }

        // ShortenPath(...) methods from http://www.codinghorror.com/blog/archives/000650.html

        /// <summary>
        /// Makes a path short using regex. File Name and root name is preserved.
        /// </summary>
        public static string ShortenPath(string path)
        {
            const string pattern = @"^(\w+:|\\)(\\[^\\]+\\[^\\]+\\).*(\\[^\\]+\\[^\\]+)$";
            const string replacement = "$1$2...$3";
            if (Regex.IsMatch(path, pattern))
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
            //		   const string pattern = @"^(http|https|ftp|file)(://[^/]+/).*(/[^\n\r\\x00?].*)$";
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

        public static bool ValidateUri(string url, out string failReason)
        {
            Uri uri;
            return ValidateAndCreateUri(url, out uri, out failReason);
        }

        public static bool ValidateAndCreateUri(string url, out Uri uri, out string failReason)
        {
            if (string.IsNullOrEmpty(url)) { failReason = "Url is null"; uri = null; return false; }
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                failReason = "Invalid or relative URI.";
                return false;
            }
            return ValidateUri(uri, out failReason);
        }

        public static bool ValidateUri(Uri uri, out string failReason)
        {
            failReason = "";

            if ((uri.Scheme != Uri.UriSchemeHttp) &&
                (uri.Scheme != Uri.UriSchemeHttps) && 
                (uri.Scheme != Uri.UriSchemeFtp) && 
                (uri.Scheme != Uri.UriSchemeFile))
            {
                failReason = "Unsupported URI scheme. Scheme: " + uri.Scheme;
                return false;
            }
            
            return true;
        }

        public static float CalculatePercent(int current, int total)
        {
            if ((total <= 0) || (current <= 0))
                return 0f;
            if (current == total)
                return 100f;

            return (float)Math.Round((float)current / total * 100f, 2);
        }

        public static float CalculatePercent(long current, long total)
        {
            if ((total <= 0) || (current <= 0))
                return 0f;
            if (current == total)
                return 100f;

            return (float)Math.Round((float)current / total * 100f, 2);
        }
    }
}
