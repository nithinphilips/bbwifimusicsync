using System;

namespace iTunesExport.Parser
{
    public interface ITrack
    {
        string Album { get; }
        string AlbumArtist { get; }
        string Artist { get; }
        bool Disabled { get; }
        string FileName { get; }
        string Genre { get; }
        int Id { get; }
        bool InLibrary { get; }
        string Location { get; }
        string Name { get; }
        int TrackTime { get; }
        int Year { get; }
    }
}
