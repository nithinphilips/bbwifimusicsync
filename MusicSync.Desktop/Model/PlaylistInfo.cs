/**********************************************************************
 * WifiMusicSync
 * Copyright (C) 2011 Nithin Philips <nithin@nithinphilips.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 **********************************************************************/

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
            set { _name = value; OnPropertyChanged("Name"); }
        }

        private bool? _checked;
        public bool? Checked
        {
            get { return _checked; }
            set { _checked = value; OnPropertyChanged("Checked"); }
        }

        private IPlaylist _playlist;
        public IPlaylist Playlist
        {
            get { return _playlist; }
            set { _playlist = value; OnPropertyChanged("Playlist"); }
        }

        private bool _existsAtDestination;
        public bool ExistsAtDestination
        {
            get { return _existsAtDestination; }
            set { _existsAtDestination = value; OnPropertyChanged("ExistsAtDestination"); }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format("[{0}] {1}", (Checked ?? false) ? "X" : " ", Name);
        }
    }
}