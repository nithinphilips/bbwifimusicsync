using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace WifiMusicSync.Helpers
{
    public class Utilities
    {
        const string NEWLINE = "\n";

        /// <summary>
        /// Calculates SHA1 hash of a string.
        /// </summary>
        /// <param name="str">The string to hash.</param>
        /// <returns>Hash in hexadecimal string.</returns>
        public static string GetSHA1Hash(string str)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retVal = sha1.ComputeHash(UTF8Encoding.UTF8.GetBytes(str));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Saves a playlist to a file.
        /// </summary>
        /// <param name="playlist">The playlist to save, as a string enumeration.</param>
        /// <param name="path">The path to save the file to.</param>
        public static void SavePlaylist(IEnumerable<string> playlist, string path)
        {
            if (File.Exists(path)) File.Delete(path);
            using (FileStream playlistFs = new FileStream(path, FileMode.CreateNew))
            {
                foreach (var item in playlist)
                {
                    Debug.Assert(item.StartsWith("file"));
                    byte[] line = Encoding.UTF8.GetBytes(item + NEWLINE);
                    playlistFs.Write(line, 0, line.Length);
                }
            }
        }

        /// <summary>
        /// Loads a playlist from a file.
        /// </summary>
        /// <param name="path">The path of the playlist file.</param>
        /// <returns>The loaded playlist as a List.</returns>
        public static List<string> LoadPlaylist(string path)
        {
            List<string> playlist = new List<string>();
            using (FileStream playlistFs = File.OpenRead(path))
            {
                using (StreamReader sr = new StreamReader(playlistFs, Encoding.UTF8))
                {
                    while (sr.Peek() > 0)
                    {
                        string line = sr.ReadLine();
                        Debug.Assert(line.StartsWith("file"));
                        playlist.Add(line);
                    }
                }
            }
            return playlist;
        }

        public static string ToBlackberryPath(string path)
        {
            string driveName = path.Substring(0, 2);
            Uri uri = new Uri(path);
            return uri.ToString().Replace(driveName, "SDCard");
        }

        public static string EscapeString(string name)
        {
            string result = Uri.EscapeUriString(name);
            result = result.Replace("&", Uri.HexEscape('&'));
            result = result.Replace(" ", Uri.HexEscape(' '));
            result = result.Replace("#", Uri.HexEscape('#'));
            return result;
        }

        public static string UnEscapeString(string name)
        {
            string result = Uri.UnescapeDataString(name);
            result = result.Replace(Uri.HexEscape('&'), "&");
            result = result.Replace(Uri.HexEscape(' '), " ");
            result = result.Replace(Uri.HexEscape('#'), "#");
            return result;
        }

        public static string MakeFileNameSafe(string name)
        {
            string result = name.Replace('/', '_');

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(invalidChar, '_');
            }

            if (result.StartsWith(".")) result = "_" + result.Substring(1);
            if (result.EndsWith(".")) result = result.Substring(0, result.Length - 1) + "_";

            return result;
        }
    }
}
