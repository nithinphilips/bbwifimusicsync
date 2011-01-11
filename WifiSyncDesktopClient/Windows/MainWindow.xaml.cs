using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WifiSyncDesktopClient.Model;
using LibQdownloader.Utilities;
using WifiMusicSync.Helpers;

namespace WifiSyncDesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SyncSettings viewModel = new SyncSettings();

        public MainWindow()
        {
            InitializeComponent();
            viewModel.LoadPlaylists();
            viewModel.Status = "Ready";
            this.DataContext = viewModel;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (viewModel.HasCapacityExceeded)
            {
                MessageBox.Show(string.Format("Current selection exceeds drive capacity by {0}", Common.ToReadableSize(-viewModel.RemainingCapacity)));
            }
            else
            {
                foreach (var item in viewModel.SelectedPlaylists)
                {
                    string playlistPath = System.IO.Path.Combine(viewModel.Path, item.Playlist.GetSafeName() + ".m3u");
                    string root = System.IO.Path.Combine(viewModel.Path, "Songs");

                    string tRoot = TransformPath(root);

                    List<string> playlist = new List<string>();
                    foreach (var track in item.Playlist.Tracks)
                    {
                        playlist.Add(track.GetPlaylistLine(tRoot));
                        //Console.WriteLine(track.GetPlaylistLine(root, System.IO.Path.DirectorySeparatorChar));
                    }
                    WifiMusicSync.Helpers.Utilities.SavePlaylist(playlist, playlistPath);
                }
            }
        }

        private string TransformPath(string path)
        {
            string driveName = path.Substring(0, 2);
            Uri uri = new Uri(path);
            return uri.ToString().Replace(driveName, "SDCard");
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            if (string.IsNullOrWhiteSpace(viewModel.Path))
            {
                e.CanExecute = false;
            }
            else if(viewModel.SelectedTracksSize <= 0)
            {
                e.CanExecute = false;
            }
        }
    }
}
