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

using System.IO;
using iTunesLib;
using libMusicSync.Helpers;
using libMusicSync.iTunesExport.Parser;
using libMusicSync.Model;
using WifiSyncServer.Model;

namespace WifiSyncServer.Extensions
{
    public static class Extensions
    {
        public static string GetSafeName(this IITPlaylist playlist)
        {
            return Helper.MakeFileNameSafe(playlist.Name);
        }

        public static string GetPlaylistLine(this IITFileOrCDTrack track, string root)
        {
            return GetPlaylistLine(track, root, '/', true);
        }

        public static string GetPlaylistLine(this IITFileOrCDTrack track, string root, char directorySeparatorChar, bool escapeString)
        {
            string separator = (root[root.Length - 1] == directorySeparatorChar) ? "" : directorySeparatorChar.ToString();
            string playlistStr = "Songs" + directorySeparatorChar.ToString();

            bool isCompilation = (!string.IsNullOrEmpty(track.AlbumArtist) && (track.AlbumArtist.Trim() != track.Artist.Trim()));

            string artist = string.IsNullOrEmpty(track.AlbumArtist) ? track.Artist : track.AlbumArtist;

            if (isCompilation) { artist = "Compilations"; }

            if (escapeString)
            {
                playlistStr += string.Format("{1}{0}{2}{0}{3}",
                                             directorySeparatorChar,
                                             Helper.EscapeString(Helper.MakeFileNameSafe(artist)),
                                             Helper.EscapeString(Helper.MakeFileNameSafe(track.Album)),
                                             Helper.EscapeString(Path.GetFileName(track.Location)));
            }
            else
            {
                playlistStr += string.Format("{1}{0}{2}{0}{3}",
                                             directorySeparatorChar,
                                             Helper.MakeFileNameSafe(artist),
                                             Helper.MakeFileNameSafe(track.Album),
                                             Path.GetFileName(track.Location));
            }

            return root + separator + playlistStr;
        }

        public static ITrack ToTrack(this IITFileOrCDTrack track)
        {
            return new Track(track.trackID, track.Name, track.Artist, track.AlbumArtist, track.Album, track.Genre, track.Year, track.Size, track.Duration, track.Location, false, !track.Enabled);
        }


        /// <summary>
        /// Unescapes the DeviceLocation property of an array of SyncActions.
        /// </summary>
        /// <param name="actions">The actions to unescape.</param>
        public static void UnEscapeAllDeviceLocations(this SyncAction[] actions)
        {
            foreach (var item in actions)
            {
                item.DeviceLocation = Helper.UnEscapeString(item.DeviceLocation);
            }
        }
    }
}
