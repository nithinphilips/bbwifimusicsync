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
using System.Diagnostics;
using System.Linq;
using iTunesLib;
using libMusicSync.iTunes;
using libMusicSync.iTunesExport.Parser;
using libMusicSync.Model;
using WifiSyncServer.Extensions;
using libMusicSync.Extensions;
using WifiSyncServer.Helpers;

namespace WifiSyncServer.iTunes
{
    public class ComiTunesLibrary : iTunesLibrary
    {
        iTunesApp app;
        Dictionary<string, IITPlaylist> playlistLookupTable = new Dictionary<string, IITPlaylist>();

        public ComiTunesLibrary()
        {
            app = new iTunesApp();
            CanModify = true;
            MusicFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            Playlists = GetPlaylists();
            //TODO: Assign Artists and Albums
        }

        /// <summary>
        /// Remove some tracks from an iTunes playlist.
        /// </summary>
        /// <param name="playlist">The playlist to modify</param>
        /// <param name="tracks">The tracks to remove from playlist.</param>
        /// <returns>An enumeration of tracks that were successfully removed from playlist.</returns>
        public IEnumerable<ITrack> RemoveTracks(IPlaylist playlist, IEnumerable<ITrack> tracks)
        {
            List<ITrack> removedtracks = new List<ITrack>();

            IITPlaylist pls = playlistLookupTable[playlist.GetSafeName()];

            foreach (IITTrack iTunesTrack in pls.Tracks)
            {
                if (!(iTunesTrack is IITFileOrCDTrack)) continue;
                IITFileOrCDTrack _item = (IITFileOrCDTrack)iTunesTrack;

                foreach (var track in tracks.Where(track => track.Location == _item.Location))
                {
                    removedtracks.Add(track);
                    iTunesTrack.Delete();
                    break;
                }
            }

            return removedtracks;
        }

        /// <summary>
        /// Adds some tracks to an iTunes playlist.
        /// </summary>
        /// <param name="playlist">The playlist to modify</param>
        /// <param name="playlistLines">The tracks to add to iTunes as represented by their device location.</param>
        /// <param name="searchHint">
        /// A playlist folder to look in. This will reduce the search time.
        /// This only works under the assumption that all music that can possibly be on the device are under this folder.
        /// Use "Music" to search all songs.
        /// </param>
        /// <param name="root">The root device path used to generate device location. This should be the same root used to generate "playlistLines"</param>
        /// <returns>An enumeration of all songs that were successfully added to the playlist.</returns>
        public IEnumerable<string> AddTracks(IPlaylist playlist, IEnumerable<string> playlistLines, ISubscription subscription, string root)
        {
            List<string> addedTracks = new List<string>();

            IITUserPlaylist targetPlaylist = playlistLookupTable[playlist.GetSafeName()] as IITUserPlaylist;

            if (targetPlaylist == null) return addedTracks;

            var subscribedTracks = GetSubscribedTracks(subscription);

            foreach (var line in playlistLines)
            {
                foreach (var iTrack in subscribedTracks)
                {
                    if (line != iTrack.GetPlaylistLine(root)) continue;

                    targetPlaylist.AddTrack(iTrack);
                    addedTracks.Add(line);
                    break;
                }    
            }

            return addedTracks;
        }

        public IEnumerable<IITFileOrCDTrack> GetSubscribedTracks(ISubscription subscription)
        {
            HashSet<IITFileOrCDTrack> tracks = new HashSet<IITFileOrCDTrack>();
            foreach (var playlist in subscription.Playlists)
            {
                IITPlaylist iPlaylist = playlistLookupTable[playlist];
                if(iPlaylist != null)
                {
                    foreach (var track in iPlaylist.Tracks.OfType<IITFileOrCDTrack>())
                    {
                        tracks.Add(track);
                    }
                }
            }
            return tracks;
        }

        /// <summary>
        /// Gets all playlists that are present in iTunes library.
        /// </summary>
        /// <remarks>
        /// The playlist will also be indexed in playlistLookupTable with IPlaylist as key and IITPlaylist as value for easy retrieval of iTunes COM objects.
        /// </remarks>
        /// <returns>An enumeration of all playlists in iTunes. </returns>
        protected IEnumerable<IPlaylist> GetPlaylists()
        {
            List<IPlaylist> playlists = new List<IPlaylist>();
            IITSource library = app.Sources.ItemByName["Library"];

            foreach (IITPlaylist item in library.Playlists)
            {
                IPlaylist pls = new Playlist(item.playlistID, item.Name, false, new IITTrackEnumerator(item));
                playlists.Add(pls);
                playlistLookupTable.Add(pls.GetSafeName(), item);
            }

            return playlists;
        }

        public IITPlaylistCollection GetiPlaylists()
        {
            IITSource library = app.Sources.ItemByName["Library"];
            return library != null ? library.Playlists : null;
        }

        public static bool IsItunesRunning()
        {
            Process[] procs = Process.GetProcessesByName("iTunes");

            return procs.Any();
        }

    }
}
