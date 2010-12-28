using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace WifiMusicSync
{
    class Utilities
    {
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

        public static void SavePlaylist(List<string> playlist, string path)
        {
            using (FileStream playlistFs = File.OpenWrite(path))
            {
                using (StreamWriter stream = new StreamWriter(playlistFs))
                {
                    foreach (var item in playlist)
                    {
                        stream.WriteLine(item);
                    }
                }
            }
        }

        public static List<string> LoadPlaylist(string path)
        {
            List<string> playlist = new List<string>();
            using (FileStream playlistFs = File.OpenRead(path))
            {
                using (StreamReader sr = new StreamReader(playlistFs))
                {
                    while (sr.Peek() > 0)
                    {
                        playlist.Add(sr.ReadLine());
                    }
                }
            }
            return playlist;
        }
    }
}
