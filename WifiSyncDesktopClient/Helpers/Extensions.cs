using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiSyncDesktopClient.Model;
using iTunesExport.Parser;
using WifiMusicSync.Helpers;
using WifiSyncDesktopClient.Threading;
using LibQdownloader.Utilities;
using System.IO;

namespace WifiSyncDesktopClient.Helpers
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
            string root = System.IO.Path.Combine(s.Path, "Songs");

            foreach (var item in s.GetSelectedTracksUnique())
            {
                string targetPath = item.GetPlaylistLine(root, System.IO.Path.DirectorySeparatorChar, false);
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

                string tRoot = Utilities.ToBlackberryPath(root);

                List<string> playlist = new List<string>();
                foreach (var track in item.Playlist.Tracks)
                {
                    playlist.Add(track.GetPlaylistLine(tRoot));
                    //Console.WriteLine(track.GetPlaylistLine(root, ));
                }
                WifiMusicSync.Helpers.Utilities.SavePlaylist(playlist, playlistPath);
            }
        }
    }
}
