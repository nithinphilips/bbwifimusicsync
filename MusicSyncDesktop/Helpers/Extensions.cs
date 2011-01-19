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
        /// <summary>
        /// Gets all selected tracks without any duplicates.
        /// </summary>
        /// <returns>Selected Tracks</returns>
        public static IEnumerable<ITrack> GetSelectedTracksUnique(this SyncSettings s)
        {
            HashSet<int> uniqueTracks = new HashSet<int>();

            foreach (var playlist in s.GetSelectedPlaylists())
            {
                foreach (var track in playlist.Playlist.Tracks)
                {
                    // Use the hash set to make sure we tally the track size only once.
                    if (!uniqueTracks.Contains(track.Id))
                    {
                        yield return track;
                    }
                }
            }
        }

        public static IEnumerable<FileCopyJob> GetSelectedTracksUniqueAsFileCopyJobs(this SyncSettings s)
        {
            foreach (var item in s.GetSelectedTracksUnique())
            {
                string targetPath = item.GetPlaylistLine(s.Path, System.IO.Path.DirectorySeparatorChar, false);
                if (!File.Exists(targetPath))
                {
                    yield return new FileCopyJob
                                     {
                                         Source = item.Location,
                                         Destination = targetPath,
                                         Size = Common.ToReadableSize(item.Size)
                                     };
                }
            }
        }

        /// <summary>
        /// Look for any (matching) existing playlists in Path and sets the Checked property of the associated playlist.
        /// </summary>
        public static void CheckExistingPlaylists(this SyncSettings s)
        {
            if (!string.IsNullOrWhiteSpace(s.Path) && Directory.Exists(s.Path))
            {
                IEnumerable<string> existingPlaylistNames = (from f in Directory.GetFiles(s.Path, "*.m3u")
                                                             select Path.GetFileNameWithoutExtension(f)).ToList();

                foreach (var librayPlaylist in s.Playlists)
                {
                    if (!librayPlaylist.Checked.HasValue) librayPlaylist.Checked = false;
                    librayPlaylist.ExistsAtDestination = false;
                    foreach (var existingPlaylist in existingPlaylistNames)
                    {                        
                        if (librayPlaylist.Playlist.GetSafeName() == existingPlaylist)
                        {
                            // Set to indeterminate only if the user has not checked it
                            if(librayPlaylist.Checked.HasValue && librayPlaylist.Checked.Value == false) librayPlaylist.Checked = null;
                            librayPlaylist.ExistsAtDestination = true;
                        }
                    }
                }
            }
        }

        public static void SaveAllM3uPlaylists(this SyncSettings s)
        {
            foreach (var item in s.GetSelectedPlaylists())
            {
                string playlistPath = System.IO.Path.Combine(s.Path, item.Playlist.GetSafeName() + ".m3u");
                string root = System.IO.Path.Combine(s.Path, "Songs");

                string tRoot = Helper.ToBlackberryPath(root);

                List<string> playlist = new List<string>();
                foreach (var track in item.Playlist.Tracks)
                {
                    playlist.Add(track.GetPlaylistLine(tRoot));
                    //Console.WriteLine(track.GetPlaylistLine(root, ));
                }
                Helper.SavePlaylist(playlist, playlistPath);
            }
        }
    }
}
