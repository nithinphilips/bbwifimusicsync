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
using WifiSyncDesktop.Helpers;

namespace WifiSyncDesktop.Model
{
    public class CopyProgressModel : INotifyPropertyChanged
    {
        private string _from;
        public string From
        {
            get { return _from; }
            set { _from = value; OnPropertyChanged("From"); }
        }

        private string _to;
        public string To
        {
            get { return _to; }
            set { _to = value; OnPropertyChanged("To"); }
        }

        private string _size;
        public string Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged("Size"); }
        }

        private int _percentage;
        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = value; OnPropertyChanged("Percentage"); }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
