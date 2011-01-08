using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace iTunesExport.Parser
{
    /// <summary>
    /// Handles the parsing duties for the iTunes XML library.
    /// </summary>
    public class LibraryParser
    {
        private string _originalMusicFolder = null;
        private string _musicFolder = null;
        private Hashtable _tracks = null;
        private Hashtable _playlists = null;

        #region Constructor

        /// <summary>
        /// Creates a new instance of LibraryParser for the iTunes XML library provided.
        /// </summary>
        /// <param name="fileLocation">The iTunes XML library to be parsed by this instance
        /// of the LibraryParser.</param>
        public LibraryParser( string fileLocation )
        {
            _tracks = new Hashtable();
            _playlists = new Hashtable();

            parseLibrary( fileLocation );
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
        public Playlist[] Playlists
        {
            get 
            {
                Playlist[] playlists = new Playlist[_playlists.Count];
                _playlists.Values.CopyTo( playlists, 0 );
                return playlists;
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Provides the default library location used by iTunes, based on the current
        /// machine's preferred music folder location.
        /// </summary>
        /// <returns>A string containing the path the default iTunes XML library location.</returns>
        public static string GetDefaultLibraryLocation()
        {
            string mymusicDataPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            return mymusicDataPath + "\\iTunes\\iTunes Music Library.xml";
        }

        #endregion

        #region Private Parse Methods

        private void parseLibrary( string fileLocation )
        {
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
                        /// Parse out the location of the music folder used by the active library.
                        _originalMusicFolder = nodeIterator.Current.Value;
                        _musicFolder = _originalMusicFolder.Replace( "file://localhost/", String.Empty );

                        /// Fix to check for UNC paths, which don't have a drive letter and need the additional
                        /// slash at the front. Thanks to Chris Jenkins for finding this one.
                        if (_musicFolder.StartsWith("/"))
                            _musicFolder = string.Format("/{0}", _musicFolder);

                        _musicFolder = HttpUtility.UrlDecode( _musicFolder );
                        _musicFolder = _musicFolder.Replace( '/', Path.DirectorySeparatorChar );
                        break;
                    }
                }
            }


            /// Can't move on if we don't know where the music is stored.
            if (_musicFolder == null)
                throw new Exception("Unable to parse Music Library element from iTunes Music Library");

            /// This query gets us down to the point in the library that contains individual track details.
            nodeIterator = xPathNavigator.Select( "/plist/dict/dict/dict" );
            while( nodeIterator.MoveNext() )
            {
                /// Parse the track details, wherein a Track reference will be added to _tracks.
                parseTrack( nodeIterator.Current.SelectChildren( XPathNodeType.All ) );
            }

            /// After tracks, we're looking at the playlists that are listed in the library.
            nodeIterator = xPathNavigator.Select ("/plist/dict/array/dict");
            while( nodeIterator.MoveNext() )
            {
                /// Parse the playlist details wherein a Playlist reference will be added to _playlists.
                parsePlaylist( nodeIterator.Current.SelectChildren( XPathNodeType.All ) );
            }
        }

        private void parseTrack( XPathNodeIterator nodeIterator )
        {
            string id = null;
            string name = null;
            string artist = null;
            string tracktime = null;
            string location = null;
            bool inLibrary = false;
            bool disabled = false;

            string currentValue;
            while( nodeIterator.MoveNext() )
            {
                currentValue = nodeIterator.Current.Value;
                if( currentValue.Equals( "Track ID" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        id = nodeIterator.Current.Value;;
                    }
                }
                else if( currentValue.Equals( "Name" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        name = nodeIterator.Current.Value;;
                    }
                }
                else if( currentValue.Equals( "Artist" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        artist = nodeIterator.Current.Value;;
                    }
                }
                else if ( currentValue.Equals( "Total Time" ) )
                {
                    if ( nodeIterator.MoveNext() )
                        tracktime = nodeIterator.Current.Value;
                }
                else if( currentValue.Equals( "Location" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        location = nodeIterator.Current.Value;
                        inLibrary = location.IndexOf( _originalMusicFolder ) != -1;
                        if( inLibrary )
                        {
                            location = location.Replace( _originalMusicFolder, String.Empty );
                        }
                        else
                        {
                            location = location.Replace( "file://localhost/", String.Empty );
                            
                            /// The _originalMusicFolder will have already been cleaned up to deal with 
                            /// UNC paths. If we're dealing with tracks in other locations, we need to look
                            /// for UNC again and clean it up. We know it's UNC if there's a slash at the front
                            /// even after stripping off the localhost string above.
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

            if (id != null && name != null && location != null && location.Length > 0 )
                _tracks.Add(id, new Track(id, name, artist, tracktime, location, inLibrary, disabled));
        }

        private void parsePlaylist( XPathNodeIterator nodeIterator )
        {
            string id = null;
            string name = null;
            bool folder = false;
            ArrayList tracks = new ArrayList();

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
                            id = nodeIterator.Current.Value;
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
                        tracks.Add( trackIterator.Current.Value );
                    }
                }
            }

            if( id != null && name != null && tracks.Count > 0 )
            {
                _playlists.Add( id, new Playlist( id, name, folder, getTracks( tracks ) ) );
            }
        }

        /// <summary>
        /// Returns an array containing a subset of all tracks, based on the passed track IDs list.       
        /// </summary>
        /// <param name="trackIds">The list of track IDs to be returned.</param>
        /// <returns>An array of Track references. If none are found, an empty array is returned.</returns>
        private Track[] getTracks( IList trackIds )
        {
            Track[] tracks = new Track[trackIds.Count];
            int index = 0;
            foreach( string id in trackIds )
            {
                tracks[index++] = (Track) _tracks[id];
            }

            return tracks;
        }

        #endregion

    }
}
