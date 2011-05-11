using System.ComponentModel;
using libMusicSync.iTunesExport.Parser;
using WifiSyncDesktop.Helpers;

namespace WifiSyncDesktop.Model
{
    
    public class PlaylistInfo : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name");}
        }

        private bool? _checked;
        public bool? Checked
        {
            get { return _checked; }
            set { _checked = value; OnPropertyChanged("Checked");}
        }

        private IPlaylist _playlist;
        public IPlaylist Playlist
        {
            get { return _playlist; }
            set { _playlist = value; OnPropertyChanged("Playlist");}
        }

        private bool _existsAtDestination;
        public bool ExistsAtDestination
        {
            get { return _existsAtDestination; }
            set { _existsAtDestination = value; OnPropertyChanged("ExistsAtDestination"); }
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", (Checked ?? false) ? "X" : " ", Name);
        }

        public void OnPropertyChanged(string name)
        {
            if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}