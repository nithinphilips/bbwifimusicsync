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
using libMusicSync.Helpers;
using libMusicSync.iTunesExport.Parser;

namespace libMusicSync.Extensions
{
    public static class Extensions
    {
        public static string GetPlaylistFileName(this IPlaylist playlist)
        {
            if (playlist.Kind == PlaylistKind.Album)
                return playlist.GetAlbumPlaylistSafeName() + ".hpl";
            else if (playlist.Kind == PlaylistKind.Artist)
                return playlist.GetArtistPlaylistSafeName() + ".hpl";
            else
                return playlist.GetSafeName() + ".m3u";
            
        }

        public static string GetSafeName(this IPlaylist playlist)
        {
            return Helper.MakeFileNameSafe(playlist.Name);
        }

        public static string GetArtistPlaylistSafeName(this IPlaylist playlist)
        {
            return Constants.ArtistPlaylistPrefix + playlist.GetSafeName();
        }

        public static string GetAlbumPlaylistSafeName(this IPlaylist playlist)
        {
            return Constants.AlbumPlaylistPrefix + playlist.GetSafeName();
        }

        public static string GetPlaylistLine(this ITrack track, string root)
        {
            return GetPlaylistLine(track, root, '/', true);
        }

        public static string GetPlaylistLine(this ITrack track, string root, char directorySeparatorChar, bool escapeString)
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

        public static long GetSize(this ITrack track)
        {
            return new FileInfo(track.Location).Length;
        }
    }
}
