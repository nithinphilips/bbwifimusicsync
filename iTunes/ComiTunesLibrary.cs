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

        public bool RemoveTrack(IPlaylist playlist, ITrack track)
        {
            IITPlaylist pls = playlistLookupTable[playlist];

            foreach (IITTrack item in pls.Tracks)
            {
                if (item is IITFileOrCDTrack)
                {
                    IITFileOrCDTrack _item = (IITFileOrCDTrack)item;

                    if (_item.Location == track.Location)
                    {
                        item.Delete();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AddTrack(IPlaylist playlist, string playlistLine, string searchHint, string root)
        {
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
                        if (_iTrack.GetPlaylistLine(root) == playlistLine)
                        {
                            targetPlaylist.AddTrack(_iTrack);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public IEnumerable<IPlaylist> GetPlaylists()
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
