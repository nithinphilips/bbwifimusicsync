using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kayak;
using Kayak.Framework;
using iTunesLib;
using System.IO;
using WifiMusicSync.Model;
using System.Diagnostics;
using WifiMusicSync.Helpers;

namespace WifiMusicSync.Wireless
{
    public class ServerService : KayakService
    {
        static Dictionary<string, string> songDb = new Dictionary<string, string>();
        static Dictionary<string, string> playlistDb = new Dictionary<string, string>();

        [Path("/")]
        [Path("/hello")]
        public object SayHello()
        {
            return new { message = "Greetings from Wonderfalls" };
        }

        [Path("/test")]
        public object Test()
        {
            return new PlaylistRequest
            {
                PlaylistDevicePath = "file:///SDCard/BlackBerry/music/Media Sync/Playlists/aa13ee3a6953da19/Stare at Ceiling.m3u",
                PlaylistData = new string[] { 
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Iron%20%26%20Wine/The%20Shepherd's%20Dog/12%20-%20Flightless%20Bird,%20American%20Mouth.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Band%20of%20Horses/Cease%20to%20Begin/03%20-%20No%20One's%20Gonna%20Love%20You.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/The%20Kooks/Konk/01%20-%20See%20The%20Sun.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Compilations/Indie%20Rock%20Playlist_%20March%202008/118%20-%20Young%20Folks.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/The%20Kooks/Konk/02%20-%20Always%20Where%20I%20Need%20To%20Be.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Various%20Artists/Blalock's%20Indie_Rock%20Playlist_%20March%20(2010)/031%20-%20The%20Morning%20Benders%20-%20Excuses.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Various%20Artists/Blalock's%20Indie_Rock%20Playlist_%20November%20(2009)/053%20-%20Thao%20%26%20The%20Get%20Down%20Stay%20Down%20-%20Know%20Better%20Learn%20Faster.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Various%20Artists/Blalock's%20Indie_Rock%20Playlist_%20January%20(2010)/087%20-%20Beach%20House%20-%20Take%20Care.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/She%20%26%20Him/Volume%20Two/07%20-%20Gonna%20Get%20Along%20Without%20You%20Now.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Various%20Artists/Blalock's%20Indie_Rock%20Playlist_%20December%20(2009)/038%20-%20Surfer%20Blood%20-%20Floating%20Vibes.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Various%20Artists/Blalock's%20Indie_Rock%20Playlist_%20December%20(2009)/011%20-%20Oh,%20Mountain%20-%20Bear's%20Beat.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Belle%20%26%20Sebastian/The%20Boy%20with%20the%20Arab%20Strap/02%20-%20Sleep%20the%20Clock%20Around.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Belle%20%26%20Sebastian/Belle%20%26%20Sebastian%20Write%20About%20Love/04%20-%20I%20Want%20the%20World%20to%20Stop.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Belle%20%26%20Sebastian/Belle%20%26%20Sebastian%20Write%20About%20Love/02%20-%20Come%20on%20Sister.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Explosions%20In%20The%20Sky/The%20Earth%20Is%20Not%20a%20Cold%20Dead%20Place/05%20-%20Your%20Hand%20In%20Mine.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Ray%20LaMontagne/Trouble/03%20-%20Hold%20You%20In%20My%20Arms.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Thao%20With%20The%20Get%20Down%20Stay%20Down/Know%20Better%20Learn%20Faster/02%20-%20Cool%20Yourself.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Thao%20With%20The%20Get%20Down%20Stay%20Down/Know%20Better%20Learn%20Faster/03%20-%20When%20We%20Swam.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Belle%20%26%20Sebastian/The%20Life%20Pursuit/02%20-%20Another%20Sunny%20Day.mp3",
                   "file:///SDCard/BlackBerry/music/Media%20Sync/Various%20Artists/Fantastic%20Mr.%20Fox/03%20-%20Mr%20Fox%20in%20the%20Fields.mp3"    
                }
            };
        }

        [Path("/songs/{id}")]
        public FileInfo GetSong(string id)
        {
            if (songDb.ContainsKey(id))
            {
                Console.WriteLine("Sending file: " + songDb[id]);
                return new FileInfo(songDb[id]);
            }
            else
            {
                Console.WriteLine("File Not Found: " + id);
                Response.SetStatusToNotFound();
                return null;
            }
        }

        [Path("/playlists/{id}")]
        public FileInfo GetPlaylist(string id)
        {
            if (playlistDb.ContainsKey(id))
            {
                return new FileInfo(playlistDb[id]);
            }
            else
            {
                Response.SetStatusToNotFound();
                return null;
            }
        }

        [Verb("POST")]
        [Verb("PUT")]
        [Path("/query")]
        public object Query([RequestBody]PlaylistRequest t)
        {
            t.CheckValidate();

            Console.WriteLine("Received Data:");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine(t);
            Console.WriteLine("---------------------------------------------");


            string deviceIdSha1 = Utilities.GetSHA1Hash(t.DeviceId ?? "");
            string playlistName = Path.GetFileNameWithoutExtension(t.PlaylistDevicePath);
            string playlistId = Utilities.GetSHA1Hash(t.PlaylistDevicePath ?? "");

            Directory.CreateDirectory(deviceIdSha1);
            string playlistPath = Path.Combine(deviceIdSha1, playlistId);

            songDb.Clear();
            playlistDb.Clear();

            List<string> reconciledPlaylist;

            // Links playlist entries with iTunes tracks.
            Dictionary<string, IITFileOrCDTrack> iTunesDb;

            // Read and sort the playlists
            Console.Write("Loading Phone Playlist...");
            List<string> devicePlaylist = new List<string>();
            foreach (var item in t.PlaylistData)
            {
                devicePlaylist.Add(PlaylistGenerator.UnEscapeString(item));
            }
            Console.WriteLine("Done");

            //BMK TODO: Read iTunes XML for headless operation.
            Console.Write("Loading iTunes Playlist ({0})...", playlistName);
            IITPlaylist iPlaylist;
            List<string> desktopPlaylist;
            if (PlaylistGenerator.TryGetiTunesPlaylist(playlistName, out iPlaylist))
            {
                desktopPlaylist = PlaylistGenerator.GeneratePlaylist(iPlaylist, t.DeviceMediaRoot, out iTunesDb);
            }
            else
            {
                Console.WriteLine("Fail. Playlist ({0}) does not exist", playlistName);
                return new SyncResponse { ErrorMessage = "Playlist does not exist", Error = 100 };

            }
            Console.WriteLine("Done");

            IEnumerable<SyncAction> desktopChanges = null; // changes that were made on the DESKTOP. Apply to DEVICE.
            IEnumerable<SyncAction> deviceChanges = null;  // changes that were made on the DEVICE. Apply to DESKTOP.


            // Read and sort the playlists
            if (File.Exists(playlistPath))
            {
                // TODO: Read iTunes XML for headless operation.
                Console.Write("Loading reference playlist ...");
                List<string> referencePlaylist = Utilities.LoadPlaylist(playlistPath);
                Console.WriteLine("Done");


                desktopChanges = DiffHandler.Diff(desktopPlaylist, referencePlaylist);
                deviceChanges = DiffHandler.Diff(devicePlaylist, referencePlaylist);

                reconciledPlaylist = new List<string>(desktopPlaylist);
                foreach (var change in deviceChanges)
                {
                    if (change.Type == SyncType.Remove)
                    {
                       reconciledPlaylist.Remove(change.DeviceLocation);
                       if (iTunesDb.ContainsKey(change.DeviceLocation))
                       {
                           Console.WriteLine("{0} {1} from iTunes Playlist", change.Type, iTunesDb[change.DeviceLocation].Location);
                       }
                       else
                       {
                           Console.WriteLine("WARNING: Asked to delete non existent iTunes track: {0}", change.DeviceLocation);
                       }
                    }
                }
            }
            else
            {
                desktopChanges = DiffHandler.Diff(desktopPlaylist, devicePlaylist);
                reconciledPlaylist = desktopPlaylist; 
            }

            // Init SongsDB
            foreach (var change in desktopChanges)
            {
                if (change.Type == SyncType.Add)
                {
                    string id = Utilities.GetSHA1Hash(change.DeviceLocation);
                    change.TrackPath = "/songs/" + id;
                    // Remember songs, we we can serve them when client requests
                    songDb.Add(id, iTunesDb[change.DeviceLocation].Location);
                }
            }

            string playlistKey = Utilities.GetSHA1Hash(playlistPath);
            Utilities.SavePlaylist(reconciledPlaylist, playlistPath);
            playlistDb.Add(playlistKey, playlistPath);

            SyncResponse syncResponse = new SyncResponse();
            syncResponse.PlaylistServerPath = "/playlists/" + playlistKey;
            syncResponse.PlaylistDevicePath = t.PlaylistDevicePath; ;
            syncResponse.Actions = desktopChanges.ToArray();

            foreach (var item in syncResponse.Actions)
            {
                item.DeviceLocation = PlaylistGenerator.UnEscapeString(item.DeviceLocation);
                Debug.Assert(item.DeviceLocation.StartsWith("file"));
            }

            Console.WriteLine("Sending Data:");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine(syncResponse);
            Console.WriteLine("---------------------------------------------");

            return syncResponse;
        }
    }
}
