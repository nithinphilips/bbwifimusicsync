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
        IEnumerable<Playlist> Playlists { get; }

        List<string> GeneratePlaylist(Playlist playlist, string root);
        Track GetTrack(string playlistLine);
    }
}
