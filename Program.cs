using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using iTunesLib;
using System.Security.Cryptography;
using Kayak.Framework;
using Kayak;

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

            //string fileName = @"M:\BlackBerry\music\Media Sync\Playlists\aa13ee3a6953da19\Stare at Ceiling.m3u";
            
            //// Links playlist entries with iTunes tracks.
            //Dictionary<string, IITFileOrCDTrack> lookupTable;

            //// Read and sort the playlists
            //Console.Write("Loading Phone Playlist...");
            //List<string> devicePlaylist = PlaylistGenerator.ReadPlaylist(File.ReadAllText(fileName));
            //Console.WriteLine("Done");

            //// TODO: Read iTunes XML for headless operation.
            //Console.Write("Loading iTunes Playlist...");
            //List<string> desktopPlaylist = PlaylistGenerator.GeneratePlaylist(Path.GetFileNameWithoutExtension(fileName), out lookupTable);
            //Console.WriteLine("Done");

            //devicePlaylist.Sort();
            //desktopPlaylist.Sort();

            //Utilities.SavePlaylist(devicePlaylist, "Device.txt");
            //Utilities.SavePlaylist(desktopPlaylist, "Desktop.txt");

            //// create diff
            ////TODO: Use diff command from cygwin instead
            //Console.Write("Diffing...");
            //IEnumerable<SyncAction> actions = DiffHandler.Diff("Diff.diff");
            //Console.WriteLine("Done");

            //foreach (var item in actions)
            //{
            //    if (item.Type == SyncType.Add)
            //    {
            //        if (lookupTable.ContainsKey(item.DeviceLocation))
            //        {
            //            item.ServerLocation = lookupTable[item.DeviceLocation].Location;
            //            item.TrackPath = "/songs/" + Utilities.GetSHA1Hash(item.DeviceLocation);
            //        }
            //    }
            //}

            //SyncInfo syncInfo = new SyncInfo();
            //syncInfo.PlaylistServerPath = "/playlists/Stare at Ceiling.m3u";
            //syncInfo.PlaylistServerPath = "file:///SDCard/BlackBerry/music/Media Sync/Playlists/aa13ee3a6953da19/Stare at Ceiling.m3u";
            //syncInfo.Actions = actions.ToArray();

            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(syncInfo, Newtonsoft.Json.Formatting.Indented));

            //foreach (var item in actions)
            //{
            //    if (item.Type == SyncType.Add)
            //    {
            //        Console.WriteLine("{0}: {1}", item.Type, item.ServerLocation);
            //    }
            //    else
            //    {
            //        Console.WriteLine("{0}: {1}", item.Type, item.DeviceLocation);   
            //    }
            //}


            var server = new KayakServer(new System.Net.IPEndPoint(0, 9000));
            var behavior = new KayakFrameworkBehavior();
            behavior.JsonMapper.SetOutputConversion<int>((i, w) => w.Write(i.ToString()));

            var framework = server.UseFramework();

            Console.WriteLine("WifiMusicSync listening on " + server.ListenEndPoint);
            Console.ReadLine();

            // unsubscribe from server (close the listening socket)
            framework.Dispose();
        }

        
    }
}
