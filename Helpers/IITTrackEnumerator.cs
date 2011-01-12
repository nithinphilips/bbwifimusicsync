using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;
using libMusicSync.iTunesExport.Parser;
using WifiMusicSync.Extensions;

namespace WifiMusicSync.Helpers
{
    public class IITTrackEnumerator : IEnumerable<ITrack>
    {

        public IITPlaylist Playlist { get; private set; }

        public IITTrackEnumerator(IITPlaylist playlist)
        {
            Playlist = playlist;
        }
             
        public IEnumerator<ITrack> GetEnumerator()
        {
            for (int i = 1; i <= Playlist.Tracks.Count; i++)
			{
                yield return ((IITFileOrCDTrack)Playlist.Tracks[i]).ToTrack();
			}
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = 1; i <= Playlist.Tracks.Count; i++)
            {
                yield return ((IITFileOrCDTrack)Playlist.Tracks[i]).ToTrack();
            }
        }
    }
}
