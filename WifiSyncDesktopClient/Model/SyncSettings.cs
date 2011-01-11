using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using WifiMusicSync.Helpers;
using iTunesExport.Parser;
using LibQdownloader.Utilities;

namespace WifiSyncDesktopClient.Model
{
    public class PlaylistInfo
    {
        public string Name { get; set; }

        bool _checked;
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                if(Settings != null) Settings.CalulatePlaylistSize();
            }
        }

        public SyncSettings Settings { get; set; }
        public IPlaylist Playlist { get; set; }


        public override string ToString()
        {
            return string.Format("[{0}] {1}", Checked ? "X" : " ", Name);
        }
    }

    public class SyncSettings : INotifyPropertyChanged
    {
        FileSystemWatcher watcher;
        XmliTunesLibraryManager xmlLibraryManager = new XmliTunesLibraryManager();

        public IEnumerable<PlaylistInfo> Playlists { get; set; }

        public void LoadPlaylists()
        {
            var playlistSelector = from t in xmlLibraryManager.Library.Playlists
                         select new PlaylistInfo
                         {
                            Name = string.Format("{0} ({1} tracks)", t.Name, t.Tracks.Count()),
                            Checked = false,
                            Playlist = t,
                            Settings = this
                         };
            Playlists = new List<PlaylistInfo>(playlistSelector);
        }

        public IEnumerable<PlaylistInfo> SelectedPlaylists
        {
            get
            {
                return from p in Playlists
                       where p.Checked
                       select p;
            }
        }

        public void CalulatePlaylistSize()
        {
            long totalTrackSize = 0;
            HashSet<int> uniqueTracks = new HashSet<int>();

            foreach (var playlist in SelectedPlaylists)
            {
                foreach (var track in playlist.Playlist.Tracks)
                {
                    // Use the hash set to make sure we tally the track size only once.
                    if (!uniqueTracks.Contains(track.Id))
                    {
                        totalTrackSize += track.Size;
                        uniqueTracks.Add(track.Id);
                    }
                }
            }
            SelectedTracksSize = totalTrackSize;
            CalculateCapacity();
        }

        public long SelectedTracksSize
        {
            get;
            set;
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
                    CalculateCapacity();
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
                    Status = "Select one or more playlists";
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

        public void WatchFileSystem()
        {
            if (watcher != null) watcher.Dispose();
            if (!string.IsNullOrEmpty(Path))
            {
                watcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(Path));
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter =
                      NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Security
                    | NotifyFilters.Size;
                watcher.Changed += watcher_Changed;
                watcher.Deleted += watcher_Changed;
                watcher.Created += watcher_Changed;
                watcher.Renamed += watcher_Renamed;
                watcher.EnableRaisingEvents = true;
            }
        }



        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if ((e.ChangeType == WatcherChangeTypes.Deleted) && (this.Path.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase)))
            {
                // The Folder we're watching was deleted. We're gonna move up the directory tree, but not read it!
                // May be notify user in the future
                string newPath = Path;

                while (true)
                {
                    newPath = System.IO.Path.GetDirectoryName(newPath);
                    if (Directory.Exists(newPath)
                     || string.IsNullOrEmpty(newPath)) // GetDirectoryName("C:\\") == ""
                    {
                        this.Path = newPath;
                        break;
                    }
                }

            }

            Update();
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            // No need to update
            //Console.WriteLine("{0}: {1} to {2}", e.ChangeType, e.OldFullPath, e.FullPath);
            if (this.Path.Equals(e.OldFullPath, StringComparison.OrdinalIgnoreCase))
            {
                // The folder we're watching has changed.
                this.Path = e.FullPath;
            }
        }

        public void Update()
        {
            Size = 0;
            if (!string.IsNullOrEmpty(Path)) // By default the Path is empty. Just leave size as 0
                Size = DirectorySizeCalculator.CalculateSize(Path);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
