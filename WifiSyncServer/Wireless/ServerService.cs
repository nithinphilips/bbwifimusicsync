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
using log4net;
using WifiSyncServer.Helpers;
using WifiSyncServer.iTunes;
using WifiSyncServer.Model;
using WifiSyncServer.Properties;
using WifiSyncServer.Extensions;

namespace WifiSyncServer.Wireless
{
    public class ServerService : KayakService
    {
        private static readonly ILog Log = LogManager.GetLogger("WifiMusicSync.Server");
        static readonly Dictionary<string, string> SongDb = new Dictionary<string, string>();
        static readonly Dictionary<string, string> PlaylistDb = new Dictionary<string, string>();
        static readonly CachedXmliTunesLibrary CachedXmlLibrary = new CachedXmliTunesLibrary();

        [Path("/")]
        [Path("/hello")]
        public object SayHello()
        {
            Log.Info("Saying hello");
            return new { Message = "Greetings from Wonderfalls" };
        }

        [Path("/songs/{id}")]
        public FileInfo GetSong(string id)
        {
            if (SongDb.ContainsKey(id))
            {
                Log.Info("Sending file: " + SongDb[id]);
                return new FileInfo(SongDb[id]);
            }
            else
            {
                Log.Warn("File Not Found: " + id);
                Response.SetStatusToNotFound();
                return null;
            }
        }

        [Path("/playlists/{id}")]
        public FileInfo GetPlaylist(string id)
        {
            if (PlaylistDb.ContainsKey(id))
            {
                Log.Info("Sending playlist: " + PlaylistDb[id]);
                return new FileInfo(PlaylistDb[id]);
            }
            else
            {
                Log.Warn("Playlist Not Found: " + id);
                Response.SetStatusToNotFound();
                return null;
            }
        }

#if DEBUG
        [Path("/getdata")]
        public PlaylistRequest GetData(string name)
        {
            Log.Info("Generating test data for playlist " + name);
            PlaylistRequest data = new PlaylistRequest
                                       {
                                           DeviceId = Guid.NewGuid().ToString("N"),
                                           DeviceMediaRoot = "file:///SDCard/Blackberry/music/WiFiSync",
                                           PlaylistDevicePath =
                                               "file:///SDCard/Blackberry/music/WiFiSync/" + name + ".m3u"
                                       };
            IPlaylist pls = CachedXmlLibrary.Library.GetFirstPlaylistByName(name);
            if(pls != null){
                data.PlaylistData = (from t in pls.Tracks
                                     select t.GetPlaylistLine(data.DeviceMediaRoot)).ToArray();
            }

            return data;
        }
#endif

        [Path("/getplaylists")]
        public object GetPlaylists()
        {
            Log.Info("Listing playlists");

            // Note: t.Tracks.Count() causes the entire library to be enumerated and
            //       when using COM this will take f-o-r-e-v-e-r. So load the XML, no 
            //       matter what. It only takes ~4 seconds for a reasonably large library (23K ct.)
            //       Plus, it caches!
            var playlistEnumerator = from playlist in CachedXmlLibrary.Library.Playlists
                                  select new { 
                                      DisplayName = playlist.Name,  
                                      Name = playlist.GetSafeName(), 
                                      TrackCount = playlist.Tracks.Count() 
                                  };

            return new { Playlists = playlistEnumerator };
        }

        [Verb("POST")]
        [Verb("PUT")]
        [Path("/subscribe")]
        public Response Subscribe([RequestBody]Subscription s)
        {
            Log.Info("Updating subscription");

            // Check for errors
            Response errorResponse;
            if (!s.CheckValidate(out errorResponse)) return errorResponse;

            // Get garbage tracks list
            SubscriptionManager man = new SubscriptionManager(s.SafeDeviceId); // Current subscription
            SyncAction[] actions = man.GetGarbageActions(CachedXmlLibrary.Library, s);
            actions.UnEscapeAllDeviceLocations();

            Log.Info("Saving subscription to disk.");
            SubscriptionManager.SaveToDisk(s);

            SyncResponse response = new SyncResponse { Actions = actions };
            Log.Debug("Sending cleanup data:" + Environment.NewLine + response.ToString());
            return response;
        }
        

        [Verb("POST")]
        [Verb("PUT")]
        [Path("/query")]
        public Response Query([RequestBody]PlaylistRequest request)
        {
            SongDb.Clear();
            PlaylistDb.Clear();

            Response errorResponse;
            if (!request.CheckValidate(out errorResponse)) return errorResponse;

            using (log4net.ThreadContext.Stacks["NDC"].Push("Client " + request.DeviceId))
            {
                Log.Info("Client " + request.DeviceId + " connected.");
                Log.Debug("Received Data:" + Environment.NewLine + request.ToString());

                string playlistName = Path.GetFileNameWithoutExtension(request.PlaylistDevicePath);
                SubscriptionManager subManager = new SubscriptionManager(request.SafeDeviceId);

                Directory.CreateDirectory(request.SafeDeviceId);
                string playlistPath = Path.Combine(request.SafeDeviceId, request.SafePlaylistDevicePath);

                List<string> reconciledPlaylist;

                // Read and sort the playlists
                Log.Info("Loading Phone Playlist...");
                List<string> devicePlaylist = new List<string>(request.PlaylistData);

                Log.InfoFormat("Loading iTunes Library ({0})...", playlistName);
                List<string> desktopPlaylist;
                iTunesLibrary library;

                // TODO: If iTunes is already running, connect to it via COM so we can add remove etc.
                if (Settings.Default.OneWaySync)
                    library = CachedXmlLibrary.Library;
                else
                    library = new ComiTunesLibrary();


                Log.InfoFormat("iTunes library (via {0}) loaded.", Settings.Default.OneWaySync ? "XML" : "COM");
                

                IPlaylist playlist = library.GetFirstPlaylistByName(playlistName);
                if (playlist != null)
                {
                    desktopPlaylist = library.GeneratePlaylist(playlist, request.DeviceMediaRoot);
                }
                else
                {
                    Log.WarnFormat("Fail. Playlist ({0}) does not exist", playlistName);
                    return new SyncResponse { ErrorMessage = "Playlist does not exist", Error = (int)SyncResponse.SyncResponseError.PlaylistNotFound };
                }
                

                IEnumerable<SyncAction> changesOnDesktop = null; // changes that were made on the DESKTOP. Apply to DEVICE.


                // Read and sort the playlists
                if (!Settings.Default.OneWaySync && File.Exists(playlistPath))
                {
                    Log.InfoFormat("Loading reference playlist {0}...", playlistPath);
                    List<string> referencePlaylist = Helper.LoadPlaylist(playlistPath);

                    changesOnDesktop = DiffHandler.Diff(desktopPlaylist, referencePlaylist);
                    IEnumerable<SyncAction> changesOnDevice = DiffHandler.Diff(devicePlaylist, referencePlaylist);  // changes that were made on the DEVICE. Apply to DESKTOP.

                    reconciledPlaylist = new List<string>(desktopPlaylist);

                    // Pick REMOVE changes, get associated tracks, and then exclude null ones.
                    var tracksToDelete = (from change in changesOnDevice
                                          where change.Type == SyncType.Remove
                                          select library.GetTrack(change.DeviceLocation)).TakeWhile(t => t != null);



                    // Pick ADD changes
                    var tracksToAdd = from change in changesOnDevice
                                      where change.Type == SyncType.Add
                                      select change.DeviceLocation;

                    ComiTunesLibrary comLibrary = library as ComiTunesLibrary;
                    using (log4net.ThreadContext.Stacks["NDC"].Push("Removing iTrack"))
                    {
                        foreach (var track in comLibrary.RemoveTracks(playlist, tracksToDelete))
                        {
                            reconciledPlaylist.Remove(track.GetPlaylistLine(request.DeviceMediaRoot));
                            Log.Info(track.Location);
                        }
                    }

                    using (log4net.ThreadContext.Stacks["NDC"].Push("Adding iTrack"))
                    {
                        foreach (var deviceLocation in comLibrary.AddTracks(playlist, tracksToAdd, "All Music", request.DeviceMediaRoot))
                        {
                            reconciledPlaylist.Add(deviceLocation);
                            Log.Info(deviceLocation);
                        }
                    }

                }
                else
                {
                    changesOnDesktop = DiffHandler.Diff(desktopPlaylist, devicePlaylist);
                    reconciledPlaylist = desktopPlaylist;
                }

                // Make sure we don't delete any tracks still present by other playlists.
                // Also, we have to use the xml library because COM is too slow.
                changesOnDesktop = subManager.FixProposedDeletes(CachedXmlLibrary.Library, changesOnDesktop);

                foreach (var change in changesOnDesktop)
                {
                    if (change.Type != SyncType.Add) continue;

                    // Remember songs, we we can serve them when client requests
                    string id = Helper.GetSha1Hash(change.DeviceLocation);
                    change.TrackPath = "/songs/" + id;
                    SongDb.Add(id, library.GetTrack(change.DeviceLocation).Location);
                }

                string playlistKey = Helper.GetSha1Hash(playlistPath);
                Helper.SavePlaylist(reconciledPlaylist, playlistPath);
                PlaylistDb.Add(playlistKey, playlistPath);

                SyncResponse syncResponse = new SyncResponse
                                                {
                                                    PlaylistServerPath = "/playlists/" + playlistKey,
                                                    PlaylistDevicePath = request.PlaylistDevicePath,
                                                    Actions = changesOnDesktop.ToArray()
                                                };
                syncResponse.Actions.UnEscapeAllDeviceLocations();
                Log.Debug("Sending Data:" + Environment.NewLine + syncResponse.ToString());

                return syncResponse;
            }
        }
    }
}
