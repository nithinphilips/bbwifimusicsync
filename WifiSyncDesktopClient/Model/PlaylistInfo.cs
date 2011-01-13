using libMusicSync.iTunesExport.Parser;
using WifiSyncDesktop.Helpers;

namespace WifiSyncDesktop.Model
{
    [NotifyPropertyChanged]
    public class PlaylistInfo
    {
        public string Name { get; set; }
        public bool? Checked { get; set; }
        public IPlaylist Playlist { get; set; }
        public bool ExistsAtDestination { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", (Checked ?? false) ? "X" : " ", Name);
        }
    }
}