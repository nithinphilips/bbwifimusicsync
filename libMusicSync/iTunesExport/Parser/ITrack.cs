// Part of iTunes Export Project <https://sourceforge.net/projects/itunesexport/>
// Modified by Nithin Philips <nithin@nithinphilips.com>

namespace libMusicSync.iTunesExport.Parser
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
        int Size { get; }
    }
}
