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
    public class PlaylistInfo : INotifyPropertyChanged
    {
        public string Name { get; set; }

        bool? _checked = false;
        public bool? Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Checked"));
                if(Settings != null) Settings.CalulatePlaylistSize();
            }
        }

        public SyncSettings Settings { get; set; }
        public IPlaylist Playlist { get; set; }

        bool existsAtDestination = false;
        public bool ExistsAtDestination
        {
            get
            {
                return existsAtDestination;
            }
            set
            {
                this.existsAtDestination = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ExistsAtDestination"));
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Checked.Value ? "X" : " ", Name);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }

    public class SyncSettings : INotifyPropertyChanged
    {
        CachedXmliTunesLibrary cachedXmlLibrary = new CachedXmliTunesLibrary();

        public IEnumerable<PlaylistInfo> Playlists { get; set; }
        public long SelectedTracksSize { get; set; }

        public IEnumerable<PlaylistInfo> GetSelectedPlaylists()
        {
            return from p in Playlists
                   where (!p.Checked.HasValue) || (p.Checked.HasValue && p.Checked.Value == true) 
                   select p;
            
        }

        public void LoadPlaylists()
        {
            var playlistSelector = from t in cachedXmlLibrary.Library.Playlists
                                   select new PlaylistInfo
                                              {
                                                  Name = string.Format("{0} ({1} tracks)", t.Name, t.Tracks.Count()),
                                                  Checked = false,
                                                  Playlist = t,
                                                  Settings = this
                                              };
            Playlists = new List<PlaylistInfo>(playlistSelector);
        }

        

        public void CalulatePlaylistSize()
        {
            if (string.IsNullOrWhiteSpace(Path)) return;

            long totalTrackSize = 0;
            HashSet<int> uniqueTracks = new HashSet<int>();
            string root = this.Path;

            foreach (var playlist in GetSelectedPlaylists())
            {
                foreach (var track in playlist.Playlist.Tracks)
                {
                    // Use the hash set to make sure we tally the track size only once.
                    if (!uniqueTracks.Contains(track.Id))
                    {
                        string targetPath = track.GetPlaylistLine(root, System.IO.Path.DirectorySeparatorChar, false);
                        if (!File.Exists(targetPath))
                        {
                            totalTrackSize += track.Size;
                        }
                        uniqueTracks.Add(track.Id);
                    }
                }
            }
            SelectedTracksSize = totalTrackSize;
            CalculateCapacity();
        }

        

        public float ImageSizePercentage
        {
            get
            {
                if (Size > Capacity)
                    return LibQdownloader.Utilities.Common.CalculatePercent(Capacity, Size);
                else
                    return LibQdownloader.Utilities.Common.CalculatePercent(Size, Capacity);
            }
        }

        public bool HasCapacityExceeded
        {
            get
            {
                return Size > Capacity;
            }
        }


        string path;
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (value != path)
                {
                    path = value;

                    this.CheckExistingPlaylists();
                    CalculateCapacity();
                    CalulatePlaylistSize();
                    PropertyChanged(this, new PropertyChangedEventArgs("Path"));
                }
            }
        }

        private void CalculateCapacity()
        {
            if (!string.IsNullOrWhiteSpace(path) && path.Length > 1)
            {
                DriveInfo di = new DriveInfo(path.Substring(0, 1));
                this.Capacity = di.TotalSize;
                this.Size = (di.TotalSize - di.AvailableFreeSpace) + SelectedTracksSize;
                if (SelectedTracksSize == 0)
                {
                    Status = "No songs to copy";
                }else
                {
                    Status = string.Format("Capacity: {0}, Free: {1}, Required: {2}", Common.ToReadableSize(Capacity), Common.ToReadableSize(di.AvailableFreeSpace), Common.ToReadableSize(SelectedTracksSize));
                }
            }
        }

        string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Status"));
            }
        }

        long size;
        public long Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value != size)
                {
                    size = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Size"));
                    PropertyChanged(this, new PropertyChangedEventArgs("RemainingCapacity"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageSizePercentage"));
                    PropertyChanged(this, new PropertyChangedEventArgs("HasCapacityExceeded"));
                }
            }
        }


        long capacity;
        public long Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                if (value != capacity)
                {
                    capacity = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Capacity"));
                    PropertyChanged(this, new PropertyChangedEventArgs("RemainingCapacity"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageSizePercentage"));
                    PropertyChanged(this, new PropertyChangedEventArgs("HasCapacityExceeded"));
                }
            }
        }

        public long RemainingCapacity
        {
            get
            {
                return capacity - size;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
