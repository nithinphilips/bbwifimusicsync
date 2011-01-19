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
using libMusicSync.iTunesExport.Parser;

namespace libMusicSync.iTunes
{
    public abstract class iTunesLibrary
    {
        /// <summary>
        ///   Keeps a table of playlist lines and associated iTunes tracks for fast lookup.
        /// </summary>
        protected Dictionary<string, ITrack> trackLookupTable = new Dictionary<string, ITrack>();

        /// <summary>
        ///   If true, the implementing class will allow adding and deleting tracks from iTunes playlists.
        /// </summary>
        public bool CanModify { get; protected set; }

        /// <summary>
        ///   If supported by an implementing class, provides the location of the iTunes music folder.
        /// </summary>
        public string MusicFolderPath { get; protected set; }

        public IEnumerable<IPlaylist> Playlists { get; protected set; }

        /// <summary>
        ///   Generates a playlist (m3u) from an iTunes playlist.
        /// </summary>
        /// <param name = "playlist">The playlist to process.</param>
        /// <param name = "root">The root folder where files are stored in the device.</param>
        /// <returns>A list of playlist lines.</returns>
        public List<string> GeneratePlaylist(IPlaylist playlist, string root)
        {
            trackLookupTable.Clear();
            List<string> result = new List<string>();
            foreach (var track in playlist.Tracks)
            {
                string bbPath = track.GetPlaylistLine(root);
                result.Add(bbPath);
                if (trackLookupTable.ContainsKey(bbPath))
                    Console.WriteLine("Warning: Duplicate track: {0}", bbPath);
                else
                    trackLookupTable.Add(bbPath, track);
            }

            return result;
        }

        /// <summary>
        ///   Looks for a playlist and returns the first one found.
        /// </summary>
        /// <param name = "name">The name of the playlist. Illegal file name characters are expected to be replaced with '_' character.</param>
        /// <returns>The playlist if found, or null if no match.</returns>
        public IPlaylist GetFirstPlaylistByName(string name)
        {
            return (from p in Playlists
                    where p.GetSafeName() == name
                    select p).FirstOrDefault();
        }

        /// <summary>
        ///   Looks for playlists that match name.
        /// </summary>
        /// <param name = "name">The name of the playlist. Illegal file name characters are expected to be replaced with '_' character.</param>
        /// <returns>An enumeration of the found playlists.</returns>
        public IEnumerable<IPlaylist> GetPlaylistsByName(string name)
        {
            return from p in Playlists
                   where p.GetSafeName() == name
                   select p;
        }

        /// <summary>
        ///   Retrieves a track based on the playlist line.
        ///   NOTE: Since looking up the entire library is costly, this method only looks for playlist used in the previous GeneratePlaylist() method.
        /// </summary>
        /// <param name = "playlistLine"></param>
        /// <returns></returns>
        public ITrack GetTrack(string playlistLine)
        {
            return trackLookupTable.ContainsKey(playlistLine) ? trackLookupTable[playlistLine] : null;
        }
    }
}