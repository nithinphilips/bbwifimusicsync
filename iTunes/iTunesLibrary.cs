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
        /// <summary>
        /// If true, the implementing class will allow adding and deleting tracks from iTunes playlists.
        /// </summary>
        public bool CanModify { get; protected set; }

        /// <summary>
        /// If supported by an implementing class, provides the location of the iTunes music folder.
        /// </summary>
        public string MusicFolderPath { get; protected set; }
        public IEnumerable<IPlaylist> Playlists { get; protected set; }
  
        /// <summary>
        /// Keeps a table of playlist lines and associated iTunes tracks for fast lookup.
        /// </summary>
        protected Dictionary<string, ITrack> trackLookupTable = new Dictionary<string, ITrack>();

        /// <summary>
        /// Generates a playlist (m3u) from an iTunes playlist.
        /// </summary>
        /// <param name="playlist">The playlist to process.</param>
        /// <param name="root">The root folder where files are stored in the device.</param>
        /// <returns>A list of playlist lines.</returns>
        public List<string> GeneratePlaylist(IPlaylist playlist, string root)
        {
            trackLookupTable.Clear();
            List<string> result = new List<string>();
            foreach (var track in playlist.Tracks)
            {
                string bbPath = track.GetPlaylistLine(root);
                result.Add(bbPath);
                if(trackLookupTable.ContainsKey(bbPath))
                    Console.WriteLine("Warning: Duplicate track: {0}", bbPath);
                else
                    trackLookupTable.Add(bbPath, track);
            }

            return result;
        }

        /// <summary>
        /// Looks for a playlist and returns the first one found.
        /// </summary>
        /// <param name="name">The name of the playlist. Illegal file name characters are expected to be replaced with '_' character.</param>
        /// <returns>The playlist if found, or null if no match.</returns>
        public IPlaylist GetFirstPlaylistByName(string name)
        {
            return (from p in Playlists
                    where p.GetSafeName() == name
                    select p).FirstOrDefault();
        }

        /// <summary>
        /// Looks for playlists that match name.
        /// </summary>
        /// <param name="name">The name of the playlist. Illegal file name characters are expected to be replaced with '_' character.</param>
        /// <returns>An enumeration of the found playlists.</returns>
        public IEnumerable<IPlaylist> GetPlaylistsByName(string name)
        {
            return from p in Playlists
                   where p.GetSafeName() == name
                   select p;
        }

        /// <summary>
        /// Retrieves a track based on the playlist line.
        /// NOTE: Since looking up the entire library is costly, this method only looks for playlist used in the previous GeneratePlaylist() method.
        /// </summary>
        /// <param name="playlistLine"></param>
        /// <returns></returns>
        public ITrack GetTrack(string playlistLine)
        {
            return trackLookupTable.ContainsKey(playlistLine) ? trackLookupTable[playlistLine] : null;
        }
    }
}
