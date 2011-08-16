// Part of iTunes Export Project <https://sourceforge.net/projects/itunesexport/>
// Modified by Nithin Philips <nithin@nithinphilips.com>

using System;
using System.Collections.Generic;

namespace libMusicSync.iTunesExport.Parser
{
    /// <summary>
    /// Represents an individual playlist from the iTunes library.
    /// </summary>
    public class Playlist : IPlaylist, IComparable<Playlist>
    {

        protected Playlist()
        {

        }

        public Playlist(int id, string name, bool folder, PlaylistKind kind, IEnumerable<ITrack> tracks)
        {
            this.Id = id;
            this.Name = name;
            this.Folder = folder;
            this.Kind = kind;
            this.Tracks = tracks;
        }

        /// <summary>
        /// The unique ID for this playlist.
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// The display name for this playlist.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Indicates whether this playlist is based on a folder in iTunes.
        /// </summary>
        public bool Folder { get; protected set; }

        /// <summary>
        /// Indicate the type of this playlist.
        /// </summary>
        public PlaylistKind Kind { get; protected set; }

        /// <summary>
        /// An array of the Tracks that appear within this playlist.
        /// </summary>
        public IEnumerable<ITrack> Tracks { get; protected set; }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(Playlist other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
