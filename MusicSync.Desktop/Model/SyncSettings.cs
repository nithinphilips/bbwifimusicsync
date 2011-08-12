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
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.IO;
using iTuner;
using libMusicSync.Extensions;
using libMusicSync.Helpers;
using libMusicSync.iTunesExport.Parser;
using LibQdownloader.Utilities;
using WifiSyncDesktop.Helpers;
using NotifyPropertyChanged;

namespace WifiSyncDesktop.Model
{
    [NotifyPropertyChanged]
    public class SyncSettings : INotifyPropertyChangedAmendment
    {
        UsbManager man = new UsbManager();

        public SyncSettings()
        {    
            
        }

        public void LoadDrives()
        {
            this.Drives = man.GetAvailableDisks();
            UpdateCurrentPath();

            man.StateChanged += man_StateChanged;
        }

        void man_StateChanged(UsbStateChangedEventArgs e)
        {
            switch (e.State)
            {
                case UsbStateChange.Added:
                    Drives.Add(e.Disk);
                    break;
                case UsbStateChange.Removing:
                case UsbStateChange.Removed:
                    Drives.Remove(e.Disk.Name);
                    break;
            }

            if (this.Path == null) UpdateCurrentPath();
        }

        void UpdateCurrentPath()
        {
            this.Path = this.Drives.FirstOrDefault();

            if (this.Path == null)
            {
                Status = "Plug in your BlackBerry or insert a memory card to begin.";
            }
            else if(string.IsNullOrEmpty(Status))
            {
                Status = "Ready.";
            }
        }

        readonly CachedXmliTunesLibrary _cachedXmlLibrary = new CachedXmliTunesLibrary();

        public IEnumerable<PlaylistInfo> GetSelectedPlaylists()
        {
            if (Playlists != null)
            {
                return from p in Playlists
                       where (!p.Checked.HasValue) || (p.Checked.Value == true)
                       select p;
            }
            
            return new List<PlaylistInfo>();
        }

        public void LoadPlaylists()
        {
            List<PlaylistInfo> result = new List<PlaylistInfo>();
            foreach (var playlist in _cachedXmlLibrary.Library.Playlists)
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
            
            // Use the hash set to make sure we tally a track's size only once.
            HashSet<int> uniqueTracks = new HashSet<int>();
           
            foreach (var track in GetSelectedPlaylists().SelectMany(playlist => playlist.Playlist.Tracks))
            {
                if(uniqueTracks.Contains(track.Id)) continue; // Skip duplicates
                totalTrackSize += track.Size; 
                uniqueTracks.Add(track.Id);
            }
            
            // Total size of all unique selected tracks.
            SelectedTracksSize = totalTrackSize;

            if (this.Path == null)
            {
                Capacity = totalTrackSize;
                Size = totalTrackSize;
                Status = string.Format("Required Space: {0}", Common.ToReadableSize(Size));
            }
            else
            {
                DriveInfo di = new DriveInfo(this.Path.Name);

                // Get the size of the MusicSync directory
                long currentSize = DirectorySizeCalculator.GetDirectorySize(this.SyncPath);

                long sizeVariance = totalTrackSize - currentSize;

                this.Capacity = di.TotalSize;
                this.Size = (di.TotalSize - di.AvailableFreeSpace) + sizeVariance;

                if( sizeVariance == 0)
                    Status = "No songs to copy."; 
                else if(sizeVariance > 0)
                    Status = string.Format("Need about {0} of space.", Common.ToReadableSize(sizeVariance, 0));
                else
                    Status = string.Format("About {0} of space will be freed.", Common.ToReadableSize(-sizeVariance, 0));
            }

            // Fake it!
            OnPropertyChanged("RemainingCapacity");
            OnPropertyChanged("ImageSizePercentage");
            OnPropertyChanged("HasCapacityExceeded");
        }

        public IEnumerable<PlaylistInfo> Playlists { get; set; }
        public string Status { get; set; }
        public long Size { get; set; }
        public long Capacity { get; set; }
        public long SelectedTracksSize { get; set; }
        public UsbDiskCollection Drives { get; set; }

        UsbDisk _path;
        public UsbDisk Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (value == _path) return;

                _path = value;
                this.CheckExistingPlaylists();
                CalculatePlaylistSize();
            }
        }

        public string SyncPath
        {
            get
            {
                return this.Path == null ? "" : System.IO.Path.Combine(Path.Name + System.IO.Path.DirectorySeparatorChar, "Blackberry", "music", "WiFiSync");
            }
        }

        public float ImageSizePercentage
        {
            get {
                return Size > Capacity ? Common.CalculatePercent(Capacity, Size) : Common.CalculatePercent(Size, Capacity);
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
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
