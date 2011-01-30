﻿/**********************************************************************
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
using iTunesLib;
using Kayak;
using Kayak.Framework;
using System.IO;
using libMusicSync.Extensions;
using libMusicSync.Helpers;
using libMusicSync.iTunes;
using libMusicSync.iTunesExport.Parser;
using libMusicSync.Model;
using log4net;
using WifiSyncServer.Helpers;
using WifiSyncServer.iTunes;
using WifiSyncServer.Model;
using WifiSyncServer.Properties;
using WifiSyncServer.Extensions;

namespace WifiSyncServer
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

            // Failure
            Log.Warn("File Not Found: " + id);
            Response.SetStatusToNotFound();
            return null;
        }

        [Path("/playlists/{id}")]
        public FileInfo GetPlaylist(string id)
        {
            if (PlaylistDb.ContainsKey(id))
            {
                string playlistPath = PlaylistDb[id];
                Log.Info("Commiting playlist as reference.");
                File.Copy(playlistPath, DataManager.GetReferencePlaylistPath(playlistPath), true);
                Log.Info("Sending playlist: " + playlistPath);
                return new FileInfo(playlistPath);
            }

            // Failure
            Log.Warn("Playlist Not Found: " + id);
            Response.SetStatusToNotFound();
            return null;
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

        #region iTunes Control Methods

        [Verb("POST")]
        [Verb("PUT")]
        [Path("/playsong")]
        public void PlaySong([RequestBody]PlaylistRequest request)
        {
            // Required parameters: DeviceId, DeviceMediaRoot, PlaylistData
            if (!Settings.Default.AllowPlayerControl || !ComiTunesLibrary.IsItunesRunning()) return;

            Subscription subscription = DataManager.GetSubscription(request.SafeDeviceId);

            ComiTunesLibrary itunes = new ComiTunesLibrary();

            foreach (var iTrack in
                itunes.GetSubscribedTracks(subscription).Where(iTrack => request.PlaylistData.Any(track => iTrack.GetPlaylistLine(request.DeviceMediaRoot) == track)))
            {
                iTrack.Play();
                return;
            }
        }

        [Path("/play")]
        public void PlayPlaylist(string playlist)
        {
            if (!Settings.Default.AllowPlayerControl || !ComiTunesLibrary.IsItunesRunning()) return;

            ComiTunesLibrary itunes = new ComiTunesLibrary();
            IITPlaylistCollection playlists = itunes.GetiPlaylists();

            if (playlists == null) return;

            foreach (IITPlaylist _playlist in playlists)
            {
                if (_playlist.GetSafeName() == playlist)
                {
                    _playlist.PlayFirstTrack();
                }
            }

        } 

        #endregion

        [Verb("POST")]
        [Verb("PUT")]
        [Path("/subscribe")]
        public Response Subscribe([RequestBody]Subscription newSubscription)
        {
            // Check for errors
            Response errorResponse;
            if (!newSubscription.CheckValidate(out errorResponse)) return errorResponse;

            SyncAction[] actions;
            // Get garbage tracks list
            Subscription oldSubscription = DataManager.GetSubscription(newSubscription.SafeDeviceId);
            if (oldSubscription != null)
            {
                Log.Info("Updating subscription");
                SubscriptionManager man = new SubscriptionManager(oldSubscription); // Current subscription
                actions = man.GetGarbageActions(CachedXmlLibrary.Library, newSubscription);
                actions.UnEscapeAllDeviceLocations();
            }
            else
            {
                Log.Info("New subscription.");
                actions = new SyncAction[] {};
            }

            foreach (var subscribedPlaylist in newSubscription.Playlists)
            {
                Log.InfoFormat("Subscribing to: {0}", subscribedPlaylist);
            }

            Log.Info("Saving subscription to disk.");
            DataManager.SaveSubscription(newSubscription);

            SyncResponse response = new SyncResponse { Actions = actions };
            Log.Debug("Sending cleanup data:" + Environment.NewLine + response);
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

            using (ThreadContext.Stacks["NDC"].Push("Client " + request.DeviceId))
            {
                Log.Info("Client " + request.DeviceId + " connected.");
                Log.Debug("Received Data:" + Environment.NewLine + request);

                string playlistName = Path.GetFileNameWithoutExtension(request.PlaylistDevicePath);

                Subscription subscription = DataManager.GetSubscription(request.SafeDeviceId);
                SubscriptionManager subManager = subscription != null ? new SubscriptionManager(subscription) : null;

                string playlistPath = DataManager.GetDevicePlaylistPath(request);

                List<string> reconciledPlaylist;

                // Read and sort the playlists
                Log.Info("Loading Phone Playlist...");
                List<string> devicePlaylist = new List<string>(request.PlaylistData);

                Log.InfoFormat("Loading iTunes Library ({0})...", playlistName);
                List<string> desktopPlaylist;

                XmliTunesLibrary library = CachedXmlLibrary.Library;


                IPlaylist playlist = library.GetFirstPlaylistByName(playlistName);
                if (playlist != null)
                {
                    desktopPlaylist = library.GeneratePlaylist(playlist, request.DeviceMediaRoot);
                }
                else
                {
                    Log.WarnFormat("Fail. Playlist ({0}) does not exist", playlistName);

                    // We're sending an error message, along with a list of track that are no longer subscribed and
                    // can be safely removed.
                    var wipeActions =
                        request.PlaylistData.Select(s => new SyncAction {DeviceLocation = s, Type = SyncType.Remove});
                    
                    if (subManager != null)
                        wipeActions = subManager.FixProposedDeletes(library, wipeActions);

                    return new SyncResponse
                               {
                                   ErrorMessage = "Playlist does not exist",
                                   Error = (int) SyncResponseError.PlaylistNotFound,
                                   PlaylistDevicePath = request.PlaylistDevicePath,
                                   Actions = wipeActions.ToArray()
                               };
                }
                

                IEnumerable<SyncAction> changesOnDesktop = DiffHandler.Diff(desktopPlaylist, devicePlaylist); // changes that were made on the DESKTOP. Apply to DEVICE.

                // Make sure we don't delete any tracks still present by other playlists.
                // Also, we have to use the xml library because COM is too slow.
                if (subManager != null)
                    changesOnDesktop = subManager.FixProposedDeletes(library, changesOnDesktop);

                // We use different files for the serving and referencing playlists because this allows us to delay 
                // commiting a playlist as reference playlist until the client has retrieved the playlist. 
                // In situations where the sync is interrupted, it can otherwise lead to the server having incorrect 
                // state information and results in improper deletion of tracks from iTunes playlist.
                string playlistRefPath = DataManager.GetReferencePlaylistPath(playlistPath);
                if (ComiTunesLibrary.IsItunesRunning() && File.Exists(playlistRefPath))
                {
                    // TODO: This leads to an unpredictable situation where the changes on the device could 
                    // either be wiped out (iTunes is not running) or synced back (iTunes is running)
		            // NOTE: If this must be done, it is also safer to check if the PlaylistData is empty,
		            // (Possibly due to a error on the device side) in that case, we'd want to bring the 
		            // device up-to-date, by setting the desktop playlist as the reference.
                    Log.InfoFormat("iTunes is running. Connecting via COM");

                    Log.InfoFormat("Loading reference playlist {0}...", playlistRefPath);
                    List<string> referencePlaylist = Helper.LoadPlaylist(playlistRefPath);

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

                    ComiTunesLibrary comLibrary = new ComiTunesLibrary();
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
                        // Warning: Subscription could be null.
                        foreach (var deviceLocation in comLibrary.AddTracks(
                            playlist, 
                            tracksToAdd, 
                            subscription ?? Subscription.GetQuickSubscription(playlistName, request.DeviceMediaRoot, request.DeviceId), 
                            request.DeviceMediaRoot))
                        {
                            reconciledPlaylist.Add(deviceLocation);
                            Log.Info(deviceLocation);
                        }
                    }

                }
                else
                {
                    reconciledPlaylist = desktopPlaylist;
                }

               
               
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