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
using libMusicSync.Extensions;
using libMusicSync.Helpers;
using libMusicSync.iTunesExport.Parser;
using WifiSyncDesktop.Model;
using WifiSyncDesktop.Threading;
using LibQdownloader.Utilities;
using System.IO;

namespace WifiSyncDesktop.Helpers
{
    public static class Extensions
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets all selected tracks without any duplicates.
        /// </summary>
        /// <returns>Selected Tracks</returns>
        public static IEnumerable<ITrack> GetSelectedTracksDistinct(this SyncSettings s)
        {
            var trackSelector = from track in s.GetAllSelectedLists().SelectMany(p => p.Playlist.Tracks)
                                select track;

            return trackSelector.Distinct();
        }

        /// <summary>
        /// Returns a set of operations, when performed, that will change the state of files at SyncSettings.Path to match the state of the selected playlists.
        /// The returned operations are either COPY or DELETE.
        /// The returned operations are ordered COPY, then DELETE.
        /// </summary>
        /// <returns>A set of operations, when performed, that will change the state of files at SyncSettings.Path to match the state of the selected playlists.</returns>
        public static IEnumerable<FileOperation> GetFileOperations(this SyncSettings s)
        {
            HashSet<string> selectedTracks = new HashSet<string>();

            foreach (var item in s.GetSelectedTracksDistinct())
            {
                string targetPath = item.GetPlaylistLine(s.SyncPath, Path.DirectorySeparatorChar, false);
                selectedTracks.Add(targetPath);

                if (File.Exists(targetPath)) continue; // Don't do unnecessary work.

                Log.DebugFormat("Preparing COPY operation: {0} to {1}", item.Location, targetPath);
                yield return new FileOperation
                {
                    OperationType = FileOperationType.Copy,
                    Source = item.Location,
                    Destination = targetPath,
                    Size = Common.ToReadableSize(item.Size)
                };
            }

            HashSet<string> existingTracks = Utilities.Utility.GetFiles(s.SyncPath, SearchOption.AllDirectories, "*.mp3", "*.m4a", "*.wma");
            existingTracks.ExceptWith(selectedTracks);

            foreach (string garbageTrack in existingTracks)
            {
                Log.DebugFormat("Preparing DELETE operation: {0}", garbageTrack);
                yield return new FileOperation
                {
                    OperationType = FileOperationType.Delete,
                    Source = garbageTrack,
                    Size = "-"
                };
            }
        }

        /// <summary>
        /// Look for any (matching) existing playlists in Path and sets the Checked property of the associated playlist.
        /// </summary>
        public static void CheckExistingPlaylists(this SyncSettings s)
        {
            if (string.IsNullOrWhiteSpace(s.SyncPath) || !Directory.Exists(s.SyncPath)) return;

            var existingPlaylistNames =
                from f in Utilities.Utility.GetFiles(s.SyncPath, SearchOption.TopDirectoryOnly, "*.m3u", "*.hpl")
                select Path.GetFileName(f);


            if (s.Playlists != null) CheckExistingPlaylists2(s.Playlists, existingPlaylistNames);
            if (s.Albums != null) CheckExistingPlaylists2(s.Albums, existingPlaylistNames);
            if (s.Artists != null) CheckExistingPlaylists2(s.Artists, existingPlaylistNames);
        }

        private static void CheckExistingPlaylists2(IEnumerable<PlaylistInfo> playlists, IEnumerable<string> existingPlaylistNames)
        {
            foreach (var librayPlaylist in playlists)
            {
                if (!librayPlaylist.Checked.HasValue) librayPlaylist.Checked = false;
                librayPlaylist.ExistsAtDestination = false;
                foreach (var existingPlaylist in existingPlaylistNames)
                {
                    if (librayPlaylist.Playlist.GetPlaylistFileName() == existingPlaylist)
                    {
                        // Set to indeterminate only if the user has not checked it
                        if(librayPlaylist.Checked.HasValue && librayPlaylist.Checked.Value == false) librayPlaylist.Checked = null;
                        librayPlaylist.ExistsAtDestination = true;
                    }
                }
            }
        }

        public static void WritePlaylistFiles(this SyncSettings s)
        {
            foreach (string file in Utilities.Utility.GetFiles(s.SyncPath, SearchOption.TopDirectoryOnly, "*.m3u"))
            {
                File.Delete(file);
            }

            foreach (string file in Utilities.Utility.GetFiles(s.SyncPath, SearchOption.TopDirectoryOnly, "*.hpl"))
            {
                File.Delete(file);
            }

            foreach (var item in s.GetAllSelectedLists())
            {
                string playlistPath = System.IO.Path.Combine(s.SyncPath, item.Playlist.GetPlaylistFileName());
                string tRoot = Helper.ToBlackberryPath(s.SyncPath);

                List<string> playlist = new List<string>();
                foreach (var track in item.Playlist.Tracks)
                {
                    playlist.Add(track.GetPlaylistLine(tRoot));
                }

                Log.Info("Writing playlist file: " + playlistPath);
                Helper.SavePlaylist(playlist, playlistPath);
            }
        }
    }
}
