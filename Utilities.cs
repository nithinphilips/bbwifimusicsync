using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace WifiMusicSync
{
    class Utilities
    {
        const string NEWLINE = "\n";

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
    }
}
