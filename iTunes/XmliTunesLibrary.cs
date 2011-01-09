using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesExport.Parser;
using WifiMusicSync.Helpers;

namespace WifiMusicSync.iTunes
{
    public class XmliTunesLibrary : IiTunesLibrary
    {
        LibraryParser parser;

        public XmliTunesLibrary()
            : this(iTunesExport.Parser.LibraryParser.GetDefaultLibraryLocation())
        { }
        
        public XmliTunesLibrary(string xmlLibraryPath)
        {
            parser = new LibraryParser(xmlLibraryPath);
            CanModify = false;
            MusicFolderPath = parser.MusicFolder;
            Playlists = parser.Playlists;
        }

        public bool CanModify { get; private set; }
        public string MusicFolderPath { get; private set; }
        public IEnumerable<IPlaylist> Playlists { get; private set; }


        Dictionary<string, ITrack> lookupTable = new Dictionary<string, ITrack>();

        public List<string> GeneratePlaylist(IPlaylist playlist, string root)
        {
            List<string> result = new List<string>();
            foreach (var track in playlist.Tracks)
            {
                string bbPath = track.GetPlaylistLine(root);
                result.Add(bbPath);
                lookupTable.Add(bbPath, track);
            }

            return result;
        }

        public IPlaylist GetPlaylistByName(string name)
        {
            foreach (var playlist in Playlists)
            {
                if (playlist.Name == name) return playlist;
            }
            return null;
        }

        public ITrack GetTrack(string playlistLine)
        {
            if (lookupTable.ContainsKey(playlistLine))
            {
                return lookupTable[playlistLine];
            }
            else
            {
                return null;
            }
        }
    }
}
