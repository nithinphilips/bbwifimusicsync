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

using System;
using System.Collections.Generic;
using System.Linq;
using Kayak;
using Kayak.Framework;
using System.IO;
using libMusicSync.Extensions;
using libMusicSync.Helpers;
using libMusicSync.iTunes;
using libMusicSync.iTunesExport.Parser;
using System.Diagnostics;
using log4net;
using WifiSyncServer.Helpers;
using WifiSyncServer.iTunes;
using WifiSyncServer.Model;
using WifiSyncServer.Properties;

namespace WifiSyncServer.Wireless
{
    public class ServerService : KayakService
    {

        private static readonly ILog log = LogManager.GetLogger("WifiMusicSync.Server");

        static Dictionary<string, string> songDb = new Dictionary<string, string>();
        static Dictionary<string, string> playlistDb = new Dictionary<string, string>();

        static CachedXmliTunesLibrary cachedXmlLibrary = new CachedXmliTunesLibrary();

        [Path("/")]
        [Path("/hello")]
        public object SayHello()
        {
            log.Info("Saying hello");
            return new { message = "Greetings from Wonderfalls" };
        }

        [Path("/songs/{id}")]
        public FileInfo GetSong(string id)
        {
            if (songDb.ContainsKey(id))
            {
                log.Info("Sending file: " + songDb[id]);
                return new FileInfo(songDb[id]);
            }
            else
            {
                log.Warn("File Not Found: " + id);
                Response.SetStatusToNotFound();
                return null;
            }
        }

        [Path("/playlists/{id}")]
        public FileInfo GetPlaylist(string id)
        {
            if (playlistDb.ContainsKey(id))
            {
                log.Info("Sending playlist: " + playlistDb[id]);
                return new FileInfo(playlistDb[id]);
            }
            else
            {
                log.Warn("Playlist Not Found: " + id);
                Response.SetStatusToNotFound();
                return null;
            }
        }

        [Path("/getdata")]
        public PlaylistRequest GetData(string name)
        {
            PlaylistRequest data = new PlaylistRequest
                                       {
                                           DeviceId = System.Guid.NewGuid().ToString("N"),
                                           DeviceMediaRoot = "file:///SDCard/Blackberry/music/WiFiSync",
                                           PlaylistDevicePath =
                                               "file:///SDCard/Blackberry/music/WiFiSync/" + name + ".m3u"
                                       };
            IPlaylist pls = cachedXmlLibrary.Library.GetFirstPlaylistByName(name);
            if(pls != null){
                data.PlaylistData = (from t in pls.Tracks
                                     select t.GetPlaylistLine(data.DeviceMediaRoot)).ToArray();
            }

            return data;
        }

        [Path("/getplaylists")]
        public object GetPlaylists()
        {
            log.Info("Listing playlists");

            // Note: t.Tracks.Count() causes the entire library to be enumerated and
            //       when using COM this will take f-o-r-e-v-e-r. So load the XML, no 
            //       matter what. It only takes ~4 seconds for a reasonably large library (23K ct.)
            //       Plus, it caches!
            var trackEnumerator = from t in cachedXmlLibrary.Library.Playlists
                                  select new { t.Name, TrackCount = t.Tracks.Count() };

            return new { Tracks = trackEnumerator };
        }
        

        [Verb("POST")]
        [Verb("PUT")]
        [Path("/query")]
        public object Query([RequestBody]PlaylistRequest request)
        {
            songDb.Clear();
            playlistDb.Clear();

            SyncResponse errorResponse = request.CheckValidate();
            if (errorResponse != null) return errorResponse;

            using (log4net.ThreadContext.Stacks["NDC"].Push("Client " + request.DeviceId))
            {
                log.Info("Client " + request.DeviceId + " connected.");
                log.Info("Received Data:" + Environment.NewLine + request.ToString());

                string playlistName = Path.GetFileNameWithoutExtension(request.PlaylistDevicePath);
                // Uniquely identifies the device. Generated on the client side on first run and persisted.
                string deviceIdSha1 = Helper.GetSha1Hash(request.DeviceId ?? "");
                // Hash of the playlist, so we can always find the right playlist
                string playlistId = Helper.GetSha1Hash(request.PlaylistDevicePath ?? "");

                Directory.CreateDirectory(deviceIdSha1);
                string playlistPath = Path.Combine(deviceIdSha1, playlistId);

                List<string> reconciledPlaylist;

                // Read and sort the playlists
                log.Info("Loading Phone Playlist...");
                List<string> devicePlaylist = new List<string>(request.PlaylistData);

                log.InfoFormat("Loading iTunes Library ({0})...", playlistName);
                List<string> desktopPlaylist;
                iTunesLibrary library;

                // TODO: Keep the libary around and only read it if the file has actually changed.
                if (Settings.Default.OneWaySync)
                    library = cachedXmlLibrary.Library;
                else
                    library = new ComiTunesLibrary();


                log.InfoFormat("iTunes library (via {0}) loaded.", Settings.Default.OneWaySync ? "XML" : "COM");
                

                IPlaylist playlist = library.GetFirstPlaylistByName(playlistName);
                if (playlist != null)
                {
                    desktopPlaylist = library.GeneratePlaylist(playlist, request.DeviceMediaRoot);
                }
                else
                {
                    log.WarnFormat("Fail. Playlist ({0}) does not exist", playlistName);
                    return new SyncResponse { ErrorMessage = "Playlist does not exist", Error = (int)SyncResponse.SyncResponseError.PlaylistNotFound };
                }
                

                IEnumerable<SyncAction> desktopChanges = null; // changes that were made on the DESKTOP. Apply to DEVICE.
                IEnumerable<SyncAction> deviceChanges = null;  // changes that were made on the DEVICE. Apply to DESKTOP.


                // Read and sort the playlists
                if (!Settings.Default.OneWaySync && File.Exists(playlistPath))
                {
                    // TODO: Read iTunes XML for headless operation.
                    log.InfoFormat("Loading reference playlist {0}...", playlistPath);
                    List<string> referencePlaylist = Helper.LoadPlaylist(playlistPath);

                    desktopChanges = DiffHandler.Diff(desktopPlaylist, referencePlaylist);
                    deviceChanges = DiffHandler.Diff(devicePlaylist, referencePlaylist);

                    reconciledPlaylist = new List<string>(desktopPlaylist);

                    // Pick REMOVE changes, get associated tracks, and then exclude null ones.
                    var tracksToDelete = (from change in deviceChanges
                                          where change.Type == SyncType.Remove
                                          select library.GetTrack(change.DeviceLocation)).TakeWhile(t => t != null);



                    // Pick ADD changes
                    var tracksToAdd = from change in deviceChanges
                                      where change.Type == SyncType.Add
                                      select change.DeviceLocation;

                    ComiTunesLibrary comLibrary = library as ComiTunesLibrary;
                    using (log4net.ThreadContext.Stacks["NDC"].Push("Removing iTrack"))
                    {
                        foreach (var track in comLibrary.RemoveTracks(playlist, tracksToDelete))
                        {
                            reconciledPlaylist.Remove(track.GetPlaylistLine(request.DeviceMediaRoot));
                            log.Info(track.Location);
                        }
                    }

                    using (log4net.ThreadContext.Stacks["NDC"].Push("Adding iTrack"))
                    {
                        foreach (var deviceLocation in comLibrary.AddTracks(playlist, tracksToAdd, "All Music", request.DeviceMediaRoot))
                        {
                            reconciledPlaylist.Add(deviceLocation);
                            log.Info(deviceLocation);
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
                        string id = Helper.GetSha1Hash(change.DeviceLocation);
                        change.TrackPath = "/songs/" + id;
                        // Remember songs, we we can serve them when client requests
                        songDb.Add(id, library.GetTrack(change.DeviceLocation).Location);
                    }
                }

                string playlistKey = Helper.GetSha1Hash(playlistPath);
                Helper.SavePlaylist(reconciledPlaylist, playlistPath);
                playlistDb.Add(playlistKey, playlistPath);

                SyncResponse syncResponse = new SyncResponse();
                syncResponse.PlaylistServerPath = "/playlists/" + playlistKey;
                syncResponse.PlaylistDevicePath = request.PlaylistDevicePath; ;
                syncResponse.Actions = desktopChanges.ToArray();

                foreach (var item in syncResponse.Actions)
                {
                    item.DeviceLocation = Helper.UnEscapeString(item.DeviceLocation);
                    Debug.Assert(item.DeviceLocation.StartsWith("file"));
                }

                log.Info("Sending Data:" + Environment.NewLine + syncResponse.ToString());

                //GC.Collect();

                return syncResponse;
            }
        }
    }
}
