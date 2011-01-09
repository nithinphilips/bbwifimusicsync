using System;
using System.IO;

namespace iTunesExport.Parser
{
    /// <summary>
    /// Represents an individual audio track from the iTunes library.
    /// </summary>
    public class Track
    {
        public Track(
            int id, string name, string artist, string albumArtist,
            string album, string genre, int year, int trackTime,  string location, bool inLibrary, bool disabled)
        {
            this.Id = id;
            this.Name = name ?? "";
            this.Artist = artist ?? "";
            this.AlbumArtist = albumArtist ?? "";
            this.Album = album ?? "";
            this.Genre = genre ?? "";
            this.Year = year;
            this.TrackTime = trackTime;
            this.Location = location;
            this.InLibrary = inLibrary;
            this.Disabled = disabled;
        }

        /// <summary>
        /// The unique ID for this track.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The display name for this track.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The filename for this track, without the full path.
        /// </summary>
        public string FileName 
        {
            get
            {
                return Path.GetFileName(Location);
            }
        }

        /// <summary>
        /// The artist performing this track.
        /// </summary>
        public string Artist { get; private set; }

        /// <summary>
        /// The album artist performing this album.
        /// </summary>
        public string AlbumArtist { get; private set; }

        /// <summary>
        /// The album to which this track belongs to.
        /// </summary>
        public string Album { get; private set; }

        /// <summary>
        /// The genre of this track.
        /// </summary>
        public string Genre { get; private set; }

        /// <summary>
        /// The year of this track.
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// The duration of the track, in milliseconds.
        /// </summary>
        public int TrackTime { get; private set; }

        /// <summary>
        /// The complete path and filename on disc for this track.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Indicates whether this track is located in the location managed
        /// by the iTunes library.
        /// </summary>
        public bool InLibrary { get; private set; }

        /// <summary>
        /// Indicates whether this track was unchecked by the user
        /// </summary>
        public bool Disabled { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", Name, Artist, AlbumArtist, Album, Genre, Year, TrackTime, Location);
        }
    }
}
