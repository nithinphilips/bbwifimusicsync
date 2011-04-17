// Part of iTunes Export Project <https://sourceforge.net/projects/itunesexport/>
// Modified by Nithin Philips <nithin@nithinphilips.com>

using System;
using System.IO;
using System.Xml.Serialization;

namespace libMusicSync.iTunesExport.Parser
{
    /// <summary>
    /// Represents an individual audio track from the iTunes library.
    /// </summary>
    [Serializable]
    public class Track : ITrack
    {
        public Track(){}

        public Track(
            int id, string name, string artist, string albumArtist,
            string album, string genre, int year, int size, int trackTime, string location, bool inLibrary, bool disabled)
        {
            this.Id = id;
            this.Name = name ?? "";
            this.Artist = artist ?? "";
            this.AlbumArtist = albumArtist ?? "";
            this.Album = album ?? "";
            this.Genre = genre ?? "";
            this.Year = year;
            this.Size = size;
            this.TrackTime = trackTime;
            this.Location = location;
            this.InLibrary = inLibrary;
            this.Disabled = disabled;
        }

        /// <summary>
        /// The unique ID for this track.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The display name for this track.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The filename for this track, without the full path.
        /// </summary>
        [XmlIgnore]
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
        public string Artist { get; set; }

        /// <summary>
        /// The album artist performing this album.
        /// </summary>
        public string AlbumArtist { get; set; }

        /// <summary>
        /// The album to which this track belongs to.
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// The genre of this track.
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// The year of this track.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The size of the track in bytes
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The duration of the track, in milliseconds.
        /// </summary>
        public int TrackTime { get; set; }

        /// <summary>
        /// The complete path and filename on disc for this track.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Indicates whether this track is located in the location managed
        /// by the iTunes library.
        /// </summary>
        public bool InLibrary { get; set; }

        /// <summary>
        /// Indicates whether this track was unchecked by the user
        /// </summary>
        public bool Disabled { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", Name, Artist, AlbumArtist, Album, Genre, Year, TrackTime, Location);
        }
    }
}
