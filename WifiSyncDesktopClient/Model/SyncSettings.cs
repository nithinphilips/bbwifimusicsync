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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using libMusicSync.Extensions;
using libMusicSync.Helpers;
using libMusicSync.iTunesExport.Parser;
using LibQdownloader.Utilities;
using WifiSyncDesktop.Helpers;

namespace WifiSyncDesktop.Model
{
    public class SyncSettings : INotifyPropertyChanged
    {
        CachedXmliTunesLibrary cachedXmlLibrary = new CachedXmliTunesLibrary();

        public IEnumerable<PlaylistInfo> GetSelectedPlaylists()
        {
            return from p in Playlists
                   where (!p.Checked.HasValue) || (p.Checked.HasValue && p.Checked.Value == true) 
                   select p;
        } 

        public void LoadPlaylists()
        {
            List<PlaylistInfo> result = new List<PlaylistInfo>();
            foreach (var playlist in cachedXmlLibrary.Library.Playlists)
            {
                PlaylistInfo playlistInfo = new PlaylistInfo
                {
                    Name = string.Format("{0} ({1} tracks)", playlist.Name, playlist.Tracks.Count()),
                    Checked = false,
                    Playlist = playlist,
                };
                // Monitor status so we can update ourselves.
                ((INotifyPropertyChanged)playlistInfo).PropertyChanged += new PropertyChangedEventHandler(SyncSettings_PropertyChanged);
                result.Add(playlistInfo);
            }
            Playlists = result;
        }

        void SyncSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Checked") CalculatePlaylistSize();
        }

        public void CalculatePlaylistSize()
        {
            long totalTrackSize = 0;
            
            // Use the hash set to make sure we tally the track size only once.
            HashSet<int> uniqueTracks = new HashSet<int>();
           
            foreach (var track in GetSelectedPlaylists().SelectMany(playlist => playlist.Playlist.Tracks))
            {
                if(uniqueTracks.Contains(track.Id)) continue; // Skip duplicates

                if (!DoesTrackExistAtDestination(track))
                    totalTrackSize += track.Size;
                 
                uniqueTracks.Add(track.Id);
            }
            SelectedTracksSize = totalTrackSize;


            if (string.IsNullOrWhiteSpace(Path) || Path.Length < 1)
            {
                Capacity = totalTrackSize;
                Size = totalTrackSize;
                Status = string.Format("Required Space: {0}", Common.ToReadableSize(Size));
            }
            else
            {
                DriveInfo di = new DriveInfo(_path.Substring(0, 1));
                this.Capacity = di.TotalSize;

                this.Size = (di.TotalSize - di.AvailableFreeSpace) + totalTrackSize;
                Status = totalTrackSize == 0 ? "No songs to copy" : string.Format("Capacity: {0}, Free: {1}, Required: {2}", Common.ToReadableSize(Capacity), Common.ToReadableSize(di.AvailableFreeSpace), Common.ToReadableSize(totalTrackSize));
            }

            // These props are calculated on demand
            OnPropertyChanged("RemainingCapacity");
            OnPropertyChanged("ImageSizePercentage");
            OnPropertyChanged("HasCapacityExceeded");
        }

        bool DoesTrackExistAtDestination(ITrack track)
        {
            if (string.IsNullOrWhiteSpace(Path) || Path.Length < 1)
                return false;

            return File.Exists(track.GetPlaylistLine(this.Path, System.IO.Path.DirectorySeparatorChar, false));
        }

        
        public IEnumerable<PlaylistInfo> Playlists { get; set; }
        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("Status");}
        }

        private long _size;
        public long Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged("Size");}
        }

        private long _capacity;
        public long Capacity
        {
            get { return _capacity; }
            set { _capacity = value; OnPropertyChanged("Capacity");}
        }

        private long _selectedTracksSize;
        public long SelectedTracksSize
        {
            get { return _selectedTracksSize; }
            set { _selectedTracksSize = value; OnPropertyChanged("SelectedTracksSize"); }
        }

        string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (value != _path)
                {
                    _path = value;
                    this.CheckExistingPlaylists();
                    CalculatePlaylistSize();
                    OnPropertyChanged("Path");
                }
            }
        }

        public float ImageSizePercentage
        {
            get
            {
                if (Size > Capacity)
                    return Common.CalculatePercent(Capacity, Size);
                else
                    return Common.CalculatePercent(Size, Capacity);
            }
        }

        public long RemainingCapacity
        {
            get { return Capacity - Size; }
        }

        public bool HasCapacityExceeded
        {
            get { return Size > Capacity; }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
