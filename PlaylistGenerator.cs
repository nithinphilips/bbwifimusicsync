using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;
using System.IO;
using System.Diagnostics;

namespace WifiMusicSync
{
    class PlaylistGenerator
    {
        public static List<string> ReadPlaylist(StringReader reader)
        {
            List<string> result = new List<string>();

            using (reader)
            {
                while (reader.Peek() > 0)
                {
                    String line = reader.ReadLine();
                    
                    result.Add(line);
                }
            }

            return result;
        }

        public static bool TryGetiTunesPlaylist(string playlistName, out IITPlaylist iPlaylist)
        {
            iTunesApp app = new iTunesApp();
            IITSource library = app.Sources.get_ItemByName("Library");
            iPlaylist = library.Playlists.get_ItemByName(playlistName);
            return (iPlaylist != null);
        }

        public static List<string> GeneratePlaylist(IITPlaylist iPlaylist, string deviceRoot, out Dictionary<string, IITFileOrCDTrack> lookupTable)
        {
            lookupTable = new Dictionary<string, IITFileOrCDTrack>();

            List<string> result = new List<string>();


            foreach (IITTrack iTrack in iPlaylist.Tracks)
            {
                string playlistStr;

                //string albumArtist = string.IsNullOrEmpty(((IITFileOrCDTrack)iTrack).AlbumArtist) ? ((IITFileOrCDTrack)iTrack).Artist : ((IITFileOrCDTrack)iTrack).AlbumArtist;
                string albumArtist = ((IITFileOrCDTrack)iTrack).AlbumArtist;

                bool isCompilation = (string.IsNullOrEmpty(albumArtist) && iTrack.Compilation);

                string artist = string.IsNullOrEmpty(((IITFileOrCDTrack)iTrack).AlbumArtist) ? ((IITFileOrCDTrack)iTrack).Artist : ((IITFileOrCDTrack)iTrack).AlbumArtist;

                if (isCompilation) { artist = "Compilations"; }

                playlistStr = string.Format("{0}/{1}/{2}",
                         MakeFileNameSafe(artist),
                         MakeFileNameSafe(iTrack.Album),
                         Path.GetFileName(((IITFileOrCDTrack)iTrack).Location));

                string playlistLine = deviceRoot + playlistStr;
                result.Add(playlistLine);
                lookupTable.Add(playlistLine, (IITFileOrCDTrack)iTrack);
            }

            return result;
        }

        public static string EscapeString(string name)
        {
            string result = Uri.EscapeUriString(name) ;
            result = result.Replace("&", Uri.HexEscape('&'));
            result = result.Replace(" ", Uri.HexEscape(' '));
            result = result.Replace("#", Uri.HexEscape('#'));
            return result;
        }

        public static string UnEscapeString(string name)
        {
            string result = Uri.UnescapeDataString(name);
            result = result.Replace(Uri.HexEscape('&'), "&");
            result = result.Replace(Uri.HexEscape(' '), " ");
            result = result.Replace(Uri.HexEscape('#'), "#");
            return result;
        }

        static string MakeFileNameSafe(string name)
        {
            string result = name.Replace('/', '_');

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(invalidChar, '_');
            }
            
            if (result.StartsWith(".")) result = "_" + result.Substring(1);
            if (result.EndsWith(".")) result = result.Substring(0, result.Length - 1) + "_";

            return result;
        }
    }
}
