using System;
using System.Collections.Generic;

namespace iTunesExport.Parser
{
    /// <summary>
    /// Represents an individual playlist from the iTunes library.
    /// </summary>
    public class Playlist
    {

        protected Playlist()
        {

        }

        public Playlist(int id, string name, bool folder, IEnumerable<Track> tracks)
        {
            this.Id = id;
            this.Name = name;
            this.Folder = folder;
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
        /// An array of the Tracks that appear within this playlist.
        /// </summary>
        public IEnumerable<Track> Tracks { get; protected set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
