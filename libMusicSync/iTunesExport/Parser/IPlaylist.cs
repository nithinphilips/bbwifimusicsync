// Part of iTunes Export Project <https://sourceforge.net/projects/itunesexport/>
// Modified by Nithin Philips <nithin@nithinphilips.com>

using System.Collections.Generic;

namespace libMusicSync.iTunesExport.Parser
{
    public interface IPlaylist
    {
        bool Folder { get; }
        int Id { get; }
        string Name { get; }
        IEnumerable<ITrack> Tracks { get; }
    }
}
