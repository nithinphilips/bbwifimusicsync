using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesExport.Parser;
using WifiMusicSync.Helpers;

namespace WifiMusicSync.iTunes
{
    public abstract class iTunesLibrary
    {
        public bool CanModify { get; protected set; }
        public string MusicFolderPath { get; protected set; }
        public IEnumerable<IPlaylist> Playlists { get; protected set; }
  
        protected Dictionary<string, ITrack> lookupTable = new Dictionary<string, ITrack>();

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

        public IPlaylist GetFirstPlaylistByName(string name)
        {
            return (from p in Playlists
                    where p.Name == name // TODO: Normalize p.Name first, because we'll have to strip Illegal filename characters
                    select p).FirstOrDefault();
        }

        public IEnumerable<IPlaylist> GetPlaylistsByName(string name)
        {
            return from p in Playlists
                   where p.Name == name // TODO: Normalize p.Name first, because we'll have to strip Illegal filename characters
                   select p;
        }

        public ITrack GetTrack(string playlistLine)
        {
            return lookupTable.ContainsKey(playlistLine) ? lookupTable[playlistLine] : null;
        }
    }
}
