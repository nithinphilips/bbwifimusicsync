using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesExport.Parser;

namespace WifiMusicSync.iTunes
{
    public interface IiTunesLibrary
    {
        bool CanModify { get; }
        string MusicFolderPath { get; }
        IEnumerable<IPlaylist> Playlists { get; }

        List<string> GeneratePlaylist(IPlaylist playlist, string root);
        ITrack GetTrack(string playlistLine);
    }
}
