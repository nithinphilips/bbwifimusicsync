using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesExport.Parser;
using iTunesLib;
using WifiMusicSync.Helpers;
using System.IO;

namespace WifiMusicSync.iTunes
{
    public class ComiTunesLibrary : iTunesLibrary
    {
        iTunesApp app;
        Dictionary<IPlaylist, IITPlaylist> playlistLookupTable = new Dictionary<IPlaylist, IITPlaylist>();

        public ComiTunesLibrary()
        {
            app = new iTunesApp();
            CanModify = true;
            MusicFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            Playlists = GetPlaylists();
        }

        /// <summary>
        /// Remove some tracks from an iTunes playlist.
        /// </summary>
        /// <param name="playlist">The playlist to modify</param>
        /// <param name="tracks">The tracks to remove from playlist.</param>
        /// <returns>An enumeration of tracks that were successfully removed from playlist.</returns>
        public IEnumerable<ITrack> RemoveTracks(IPlaylist playlist, IEnumerable<ITrack> tracks)
        {
            List<ITrack> removedtracks = new List<ITrack>();

            IITPlaylist pls = playlistLookupTable[playlist];

            foreach (IITTrack item in pls.Tracks)
            {
                if (item is IITFileOrCDTrack)
                {
                    IITFileOrCDTrack _item = (IITFileOrCDTrack)item;

                    foreach (var track in tracks)
                    {
                        if (_item.Location == track.Location)
                        {
                            removedtracks.Add(track);
                            item.Delete();
                            break;
                        }    
                    }
                }
            }

            return removedtracks;
        }

        /// <summary>
        /// Adds some tracks to an iTunes playlist.
        /// </summary>
        /// <param name="playlist">The playlist to modify</param>
        /// <param name="playlistLines">The tracks to add to iTunes as represented by their device location.</param>
        /// <param name="searchHint">
        /// A playlist folder to look in. This will reduce the search time.
        /// This only works under the assumption that all music that can possibly be on the device are under this folder.
        /// Use "Music" to search all songs.
        /// </param>
        /// <param name="root">The root device path used to generate device location. This should be the same root used to generate "playlistLines"</param>
        /// <returns>An enumeration of all songs that were successfully added to the playlist.</returns>
        public IEnumerable<string> AddTracks(IPlaylist playlist, IEnumerable<string> playlistLines, string searchHint, string root)
        {
            List<string> addedTracks = new List<string>();

            IPlaylist hintPlaylist = this.GetFirstPlaylistByName(searchHint);
            
            if (hintPlaylist != null)
            {
                IITPlaylist searchPlaylist = playlistLookupTable[hintPlaylist];
                IITUserPlaylist targetPlaylist = playlistLookupTable[playlist] as IITUserPlaylist;

                foreach (IITTrack iTrack in searchPlaylist.Tracks)
                {
                    if (iTrack is IITFileOrCDTrack)
                    {
                        IITFileOrCDTrack _iTrack = iTrack as IITFileOrCDTrack;
                        foreach (var playlistLine in playlistLines)
                        {
                            if (_iTrack.GetPlaylistLine(root) == playlistLine)
                            {
                                targetPlaylist.AddTrack(_iTrack);
                                addedTracks.Add(playlistLine);
                                break; // Avoid 
                            }    
                        }
                    }
                }
            }

            return addedTracks;
        }

        /// <summary>
        /// Gets all playlists that are present in iTunes library.
        /// </summary>
        /// <remarks>
        /// The playlist will also be indexed in playlistLookupTable with IPlaylist as key and IITPlaylist as value for easy retrieval of iTunes COM objects.
        /// </remarks>
        /// <returns>An enumeration of all playlists in iTunes. </returns>
        protected IEnumerable<IPlaylist> GetPlaylists()
        {
            List<IPlaylist> playlists = new List<IPlaylist>();
            IITSource library = app.Sources.get_ItemByName("Library");

            foreach (IITPlaylist item in library.Playlists)
            {
                IPlaylist pls = new Playlist(item.playlistID, item.Name, false, new IITTrackEnumerator(item));
                playlists.Add(pls);
                playlistLookupTable.Add(pls, item);
            }

            return playlists;
        }

    }
}
