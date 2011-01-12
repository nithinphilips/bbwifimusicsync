using System;
using System.Collections.Generic;

namespace iTunesExport.Parser
{
    public interface IPlaylist
    {
        bool Folder { get; }
        int Id { get; }
        string Name { get; }
        IEnumerable<ITrack> Tracks { get; }
    }
}
