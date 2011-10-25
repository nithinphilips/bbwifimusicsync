// Part of iTunes Export Project <https://sourceforge.net/projects/itunesexport/>
// Modified by Nithin Philips <nithin@nithinphilips.com>

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Diagnostics;
using libMusicSync.Helpers;
using log4net;

namespace libMusicSync.iTunesExport.Parser
{
    /// <summary>
    /// Handles the parsing duties for the iTunes XML library.
    /// </summary>
    public class LibraryParser
    {
        private static readonly ILog Log = LogManager.GetLogger("LibMusicSync");

        private string _originalMusicFolder = null;
        private string _musicFolder = null;
        private Dictionary<int, ITrack> _tracks = null;
        private Dictionary<int, IPlaylist> _playlists = null;

        #region Constructor

        /// <summary>
        /// Creates a new instance of LibraryParser for the iTunes XML library provided.
        /// </summary>
        /// <param name="fileLocation">The iTunes XML library to be parsed by this instance
        /// of the LibraryParser.</param>
        public LibraryParser( string fileLocation )
        {
            _tracks = new Dictionary<int, ITrack>();
            _playlists = new Dictionary<int, IPlaylist>();

            ParseLibrary( fileLocation );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Provides the path to the Music Folder used by the current iTunes XML library.
        /// </summary>
        public string MusicFolder
        {
            get { return _musicFolder; }
        }

        /// <summary>
        /// An array of Playlist references, representing the playlists found in the current
        /// iTunes XML library.
        /// </summary>
        public IEnumerable<IPlaylist> Playlists
        {
            get 
            {
                return _playlists.Values;
            }
        }

        #endregion

        public IEnumerable<IPlaylist> GetArtists()
        {
            Dictionary<string, List<ITrack>> artistView = new Dictionary<string, List<ITrack>>();

            foreach (var track in _tracks.Values)
            {
                if (artistView.ContainsKey(track.AlbumArtist))
                    artistView[track.AlbumArtist].Add(track);
                else
                    artistView.Add(track.AlbumArtist, new List<ITrack> {track});
            }

            var list =
                artistView.Keys.Select(
                    artist => new Playlist(-1, artist, false, PlaylistKind.Artist, artistView[artist])).ToList();
            list.Sort();
            return list;
        }


        public IEnumerable<IPlaylist> GetAlbums()
        {
            Dictionary<string, List<ITrack>> albumView = new Dictionary<string, List<ITrack>>();

            foreach (var track in _tracks.Values)
            {
                if (albumView.ContainsKey(track.Album))
                    albumView[track.Album].Add(track);
                else
                    albumView.Add(track.Album, new List<ITrack> { track });
                
            }

            var list = albumView.Keys.Select(album => new Playlist(-1, album, false, PlaylistKind.Album, albumView[album])).ToList();
            list.Sort();
            return list;
        }

        #region Public Static Methods

        /// <summary>
        /// Provides the default library location used by iTunes, based on the current
        /// machine's preferred music folder location.
        /// </summary>
        /// <returns>A string containing the path the default iTunes XML library location.</returns>
        public static string GetDefaultLibraryLocation()
        {
            return ConfigReader.GetiTunesLibraryPath();
        }

        #endregion

        #region Private Parse Methods

        private void ParseLibrary( string fileLocation )
        {
            Log.Info("Parsing library file: " + fileLocation);

            StreamReader stream = new StreamReader(fileLocation, System.Text.Encoding.GetEncoding("utf-8"));
            XmlTextReader xmlReader = new XmlTextReader(stream);
            xmlReader.XmlResolver = null;
            XPathDocument xPathDocument = new XPathDocument(xmlReader);
            XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();

            XPathNodeIterator nodeIterator = xPathNavigator.Select( "/plist/dict" );
            nodeIterator.MoveNext();
            nodeIterator = nodeIterator.Current.SelectChildren( XPathNodeType.All );
            while( nodeIterator.MoveNext() )
            {
                if( nodeIterator.Current.Value.Equals( "Music Folder" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        // Parse out the location of the music folder used by the active library.
                        _originalMusicFolder = nodeIterator.Current.Value;
                        _musicFolder = _originalMusicFolder.Replace( "file://localhost/", String.Empty );

                        // Fix to check for UNC paths, which don't have a drive letter and need the additional
                        // slash at the front. Thanks to Chris Jenkins for finding this one.
                        if (_musicFolder.StartsWith("/"))
                            _musicFolder = string.Format("/{0}", _musicFolder);

                        _musicFolder = HttpUtility.UrlDecode( _musicFolder );
                        _musicFolder = _musicFolder.Replace( '/', Path.DirectorySeparatorChar );
                        break;
                    }
                }
            }


            // Can't move on if we don't know where the music is stored.
            if (_musicFolder == null)
                throw new Exception("Unable to parse Music Library element from iTunes Music Library");

            Log.Info("Using Music folder: " + _musicFolder);

            // This query gets us down to the point in the library that contains individual track details.
            nodeIterator = xPathNavigator.Select( "/plist/dict/dict/dict" );
            while( nodeIterator.MoveNext() )
            {
                // Parse the track details, wherein a Track reference will be added to _tracks.
                ParseTrack( nodeIterator.Current.SelectChildren( XPathNodeType.All ) );
            }

            // After tracks, we're looking at the playlists that are listed in the library.
            nodeIterator = xPathNavigator.Select ("/plist/dict/array/dict");
            while( nodeIterator.MoveNext() )
            {
                // Parse the playlist details wherein a Playlist reference will be added to _playlists.
                ParsePlaylist( nodeIterator.Current.SelectChildren( XPathNodeType.All ) );
            }
        }

        private void ParseTrack( XPathNodeIterator nodeIterator )
        {
            int id = -1;
            string name = null;
            string artist = null;
            int tracktime = -1;
            string location = null;
            bool inLibrary = false;
            bool disabled = false;

            string albumArtist = null;
            string album = null;
            string genre = null;
            int year = -1;
            int size = 0;


            string currentValue;
            while( nodeIterator.MoveNext() )
            {
                currentValue = nodeIterator.Current.Value;
                if( currentValue.Equals( "Track ID" ) )
                {
                    if (nodeIterator.MoveNext())
                    {
                        if (!int.TryParse(nodeIterator.Current.Value, out id))
                        {
                            Log.Error("Error parsing integer value: " + nodeIterator.Current.Value);
                        }
                    }
                }
                else if( currentValue.Equals( "Name" ) )
                {
                    if( nodeIterator.MoveNext() ) 
                        name = nodeIterator.Current.Value;
                }
                else if( currentValue.Equals( "Artist" ) )
                {
                    if( nodeIterator.MoveNext() )
                        artist = nodeIterator.Current.Value;
                }
                else if (currentValue.Equals("Album Artist"))
                {
                    if (nodeIterator.MoveNext())
                        albumArtist = nodeIterator.Current.Value;
                }
                else if (currentValue.Equals("Album"))
                {
                    if (nodeIterator.MoveNext())
                        album = nodeIterator.Current.Value;
                }
                else if (currentValue.Equals("Genre"))
                {
                    if (nodeIterator.MoveNext())
                        genre = nodeIterator.Current.Value;
                }
                else if (currentValue.Equals("Year"))
                {
                    if (nodeIterator.MoveNext())
                    {
                        if (!int.TryParse(nodeIterator.Current.Value, out year))
                        {
                            Log.Error("Error parsing integer value: " + nodeIterator.Current.Value);
                        }
                    }
                }
                else if (currentValue.Equals("Size"))
                {
                    if (nodeIterator.MoveNext())
                    {
                        if (!int.TryParse(nodeIterator.Current.Value, out size))
                        {
                            Log.Error("Error parsing integer value: " + nodeIterator.Current.Value);
                        }
                    }
                }
                else if ( currentValue.Equals( "Total Time" ) )
                {
                    if (nodeIterator.MoveNext())
                    {
                        if (!int.TryParse(nodeIterator.Current.Value, out tracktime))
                        {
                            Log.Error("Error parsing integer value: " + nodeIterator.Current.Value);
                        }
                    }
                }
                else if( currentValue.Equals( "Location" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        location = nodeIterator.Current.Value;
                        inLibrary = location.IndexOf( _originalMusicFolder ) != -1;
                        if( inLibrary )
                        {
                            // We are replacing the file://localhost/C:/Music/Folder with C:/Music/Folder
                            location = location.Replace( _originalMusicFolder, _musicFolder );
                        }
                        else
                        {
                            location = location.Replace( "file://localhost/", String.Empty );
                            
                            // The _originalMusicFolder will have already been cleaned up to deal with 
                            // UNC paths. If we're dealing with tracks in other locations, we need to look
                            // for UNC again and clean it up. We know it's UNC if there's a slash at the front
                            // even after stripping off the localhost string above.
                            if (location.StartsWith("/"))
                                location = string.Format("/{0}", location);
                        }

                        // Convert + signs to correct HTML Codes so they survive the call to UrlDecode
                        location = location.Replace( "+", "%2B" );

                        location = HttpUtility.UrlDecode( location );
                        if( location.Length > 0 && location[location.Length - 1] == '/' )
                        {
                            location = location.Substring( 0, location.Length - 1 );
                        }
                        location = location.Replace( '/', Path.DirectorySeparatorChar );
                    }
                }
                else if (currentValue.Equals("Disabled"))
                {
                    disabled = true;
                }
            }

            if (id != -1 && name != null && location != null && location.Length > 0)
            {
                _tracks.Add(id, new Track(id, name, artist, albumArtist, album, genre, year, size, tracktime, location, inLibrary, disabled));
            }
        }

        private void ParsePlaylist( XPathNodeIterator nodeIterator )
        {
            int id = -1;
            string name = null;
            bool folder = false;
            List<int> tracks = new List<int>();

            string currentName;
            string currentValue;
            while( nodeIterator.MoveNext() )
            {
                currentName = nodeIterator.Current.Name;
                if( currentName.Equals( "key" ) )
                {
                    currentValue = nodeIterator.Current.Value;
                    if( currentValue.Equals( "Name" ) )
                    {
                        if( nodeIterator.MoveNext() )
                        {
                            name = nodeIterator.Current.Value;
                        }
                    }
                    else if( currentValue.Equals( "Playlist ID" ) )
                    {
                        if( nodeIterator.MoveNext() )
                        {
                            if (!int.TryParse(nodeIterator.Current.Value, out id))
                            {
                                Log.Error("Error parsing integer value: " + nodeIterator.Current.Value);
                            }
                        }
                    }
                    else if( currentValue.Equals( "Folder" ) )
                    {
                        if( nodeIterator.MoveNext() )
                        {
                            folder = Boolean.Parse( nodeIterator.Current.Name );
                        }
                    }
                }
                else if( currentName.Equals( "array" ) )
                {
                    XPathNodeIterator trackIterator = nodeIterator.Current.Select( "dict/integer" );
                    while( trackIterator.MoveNext() )
                    {
                        int trackId = -1;
                        if (int.TryParse(trackIterator.Current.Value, out trackId))
                        {
                            tracks.Add(trackId);
                        }
                        else
                        {
                            Log.Error("Error parsing integer value: " + trackIterator.Current.Value);
                        }
                    }
                }
            }

            if( id != -1 && name != null && tracks.Count > 0 )
            {
                _playlists.Add( id, new Playlist( id, name, folder, PlaylistKind.Playlist, GetTracks( tracks ) ) );
            }
        }

        /// <summary>
        /// Returns an array containing a subset of all tracks, based on the passed track IDs list.       
        /// </summary>
        /// <param name="trackIds">The list of track IDs to be returned.</param>
        /// <returns>An array of Track references. If none are found, an empty array is returned.</returns>
        private ITrack[] GetTracks( List<int> trackIds )
        {
            ITrack[] tracks = new ITrack[trackIds.Count];
            int index = 0;
            foreach( int id in trackIds )
            {
                tracks[index++] = _tracks[id];
            }

            return tracks;
        }

        #endregion

    }
}
