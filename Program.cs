using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using iTunesLib;
using System.Security.Cryptography;

namespace WifiMusicSync
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Basic Operation:
             * 1. Listen on port 8000 for http connection
             * 
             * For each playlist:
             * 2. PHONE posts "xyz.m3u" 
             * 3. SERVER Infer the name of the playlist from the file name.
             * 4. SERVER look for the playlist in iTunes and generate an m3u file.
             * 5. SERVER sort and then diff Phone playlist and iTunes generated playlist.
             * 6. SERVER respond with Json to the phone instructing it what to do:
             *      {
             *          "Server" : "wirelessSyncServer",
             *          "Playlist": "http://192.168.0.104:8000/xyz.m3u" ,
             *          "Get" : 
             *              [ 
             *                  { 
             *                      "RemoteId" : "http://192.168.0.104:8000/songs/iTunesID",
             *                      "LocalPath" : "file://....."
             *                  },
             *                  { 
             *                      "RemoteId" : "http://192.168.0.104:8000/songs/iTunesID",
             *                      "LocalPath" : "file://....."
             *                  }
             *              ],
             *              
             *          "Delete" : 
             *              [ 
             *                  { 
             *                      "LocalPath" : "file://....."
             *                  },
             *                  { 
             *                      "LocalPath" : "file://....."
             *                  }
             *              ]
             *      }
             * 7. PHONE parses JSON
             * 8. PHONE Downloads remote songs to phone.
             * 9. PHONE Deletes removed songs.
             * 10. PHONE Dowloads new playlist and replaces the old one.
             * 11. Hopefully music player will auto update.
             */


            // Possible improvements: Phone keeps an MD5 key that the server gives 
            //                        and when requesting updates sends it to the server
            //                        and the server responds whether the playlist changed or not.

            // For JSON see <http://james.newtonking.com/projects/json-net.aspx>

            string fileName = @"M:\BlackBerry\music\Media Sync\Playlists\aa13ee3a6953da19\Stare at Ceiling.m3u";
            
            // Links playlist entries with iTunes tracks.
            Dictionary<string, IITFileOrCDTrack> lookupTable;

            // Read and sort the playlists
            Console.Write("Loading Phone Playlist...");
            List<string> devicePlaylist = PlaylistGenerator.ReadPlaylist(File.ReadAllText(fileName));
            Console.WriteLine("Done");

            Console.Write("Loading iTunes Playlist...");
            List<string> desktopPlaylist = PlaylistGenerator.GeneratePlaylist(Path.GetFileNameWithoutExtension(fileName), out lookupTable);
            Console.WriteLine("Done");

            devicePlaylist.Sort();
            desktopPlaylist.Sort();

            SavePlaylist(devicePlaylist, "Device.txt");
            SavePlaylist(desktopPlaylist, "Desktop.txt");

            // create diff
            //TODO: Use diff command from cygwin instead
            Console.WriteLine("Checking diff...");
            string parameters = string.Format("/createunifieddiff /origfile:{0} /modifiedfile:{1} /outfile:{2}", "Device.txt", "Desktop.txt", "Diff.diff");
            ProcessStartInfo si = new ProcessStartInfo("TortoiseMerge", parameters);
            si.CreateNoWindow = true;
            si.WindowStyle = ProcessWindowStyle.Minimized;
            Process.Start(si).WaitForExit();

            IEnumerable<SyncAction> actions = DiffHandler.ReadDiff("Diff.diff");

            foreach (var item in actions)
            {
                if (item.Type == SyncType.Add)
                {
                    if (lookupTable.ContainsKey(item.RemotePath))
                    {
                        item.LocalPath = lookupTable[item.RemotePath].Location;
                        item.Id = GetSHA1Hash(item.RemotePath);
                    }
                }
            }

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(actions, Newtonsoft.Json.Formatting.Indented));

            foreach (var item in actions)
            {
                if (item.Type == SyncType.Add)
                {
                    Console.WriteLine("{0}: {1}", item.Type, item.LocalPath);
                }
                else
                {
                    Console.WriteLine("{0}: {1}", item.Type, item.RemotePath);   
                }
            }

        }

        static string GetSHA1Hash(string str)
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

        private static void SavePlaylist(List<string> playlist, string path)
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
    }
}
