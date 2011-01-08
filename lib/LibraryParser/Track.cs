using System;
using System.IO;

namespace iTunesExport.Parser
{
    /// <summary>
    /// Represents an individual audio track from the iTunes library.
    /// </summary>
    public class Track
    {
        private string _id;
        private string _name;
        private string _artist;
        private int _trackTime;
        private string _location;
        private bool _inLibrary;
        private bool _disabled;


        public Track( string id, string name, string artist, string trackTime, string location, bool inLibrary, bool disabled )
        {
            _id = id;
            _name = name != null ? name : "";
            _artist = artist != null ? artist : "";
            int.TryParse(trackTime, out _trackTime);
            _location = location;
            _inLibrary = inLibrary;
            _disabled = disabled;
        }

        /// <summary>
        /// The unique ID for this track.
        /// </summary>
        public string Id
        {
            get{ return _id; }
        }

        /// <summary>
        /// The display name for this track.
        /// </summary>
        public string Name
        {
            get{ return _name; }
        }

        /// <summary>
        /// The filename for this track, without the full path.
        /// </summary>
        public string FileName
        {
            get
            {
                int index = _location.LastIndexOf(Path.DirectorySeparatorChar);
                if( index == -1 )
                {
                    return Location;
                }
                else
                {
                    return _location.Substring( index + 1 );
                }
            }
        }

        /// <summary>
        /// The artist performing this track.
        /// </summary>
        public string Artist
        {
            get{ return _artist; }
        }

        /// <summary>
        /// The duration of the track, in seconds.
        /// </summary>
        public int TrackTime
        {
            get { return _trackTime / 1000; }
        }

        /// <summary>
        /// The complete path and filename on disc for this track.
        /// </summary>
        public string Location
        {
            get{ return _location; }
        }

        /// <summary>
        /// Indicates whether this track is located in the location managed
        /// by the iTunes library.
        /// </summary>
        public bool InLibrary
        {
            get{ return _inLibrary; }
        }

        public bool Disabled
        {
            get { return _disabled; }
        }
    }
}
