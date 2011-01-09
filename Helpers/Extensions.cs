using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesExport.Parser;
using System.IO;
using iTunesLib;

namespace WifiMusicSync.Helpers
{
    public static class Extensions
    {
        public static string GetPlaylistLine(this ITrack track, string root)
        {
            string playlistStr;

            bool isCompilation = (!string.IsNullOrEmpty(track.AlbumArtist) && (track.AlbumArtist.Trim() != track.Artist.Trim()));

            string artist = string.IsNullOrEmpty(track.AlbumArtist) ? track.Artist : track.AlbumArtist;

            if (isCompilation) { artist = "Compilations"; }

            playlistStr = string.Format("{0}/{1}/{2}",
                     Utilities.MakeFileNameSafe(artist),
                     Utilities.MakeFileNameSafe(track.Album),
                     Path.GetFileName(track.Location));

            return root + playlistStr;
        }

        public static string GetPlaylistLine(this IITFileOrCDTrack track, string root)
        {
            string playlistStr;

            bool isCompilation = (!string.IsNullOrEmpty(track.AlbumArtist) && (track.AlbumArtist.Trim() != track.Artist.Trim()));

            string artist = string.IsNullOrEmpty(track.AlbumArtist) ? track.Artist : track.AlbumArtist;

            if (isCompilation) { artist = "Compilations"; }

            playlistStr = string.Format("{0}/{1}/{2}",
                     Utilities.MakeFileNameSafe(artist),
                     Utilities.MakeFileNameSafe(track.Album),
                     Path.GetFileName(track.Location));

            return root + playlistStr;
        }

        public static ITrack ToTrack(this IITFileOrCDTrack track)
        {
            return new Track(track.trackID, track.Name, track.Artist, track.AlbumArtist, track.Album, track.Genre, track.Year, track.Duration, track.Location, false, !track.Enabled);
        }
    }
}
