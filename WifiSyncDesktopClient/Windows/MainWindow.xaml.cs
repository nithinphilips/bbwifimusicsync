using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WifiSyncDesktop.Helpers;
using WifiSyncDesktop.Model;
using LibQdownloader.Utilities;
using WifiSyncDesktopClient.Threading;

namespace WifiSyncDesktopClient.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SyncSettings viewModelSync = new SyncSettings();
        CopyProgressModel viewModelCopy = new CopyProgressModel();
        FileCopyManager copyMan = new FileCopyManager();

        public int Total { get; set; }
        public int Current { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            viewModelSync.LoadPlaylists();
            viewModelSync.Status = "Ready";
            //viewModelSync.Path = @"H:\Blackberry\music";
            this.DataContext = viewModelSync;

            this.pnlProgress.DataContext = viewModelCopy;

            copyMan.JobStarting += new EventHandler<LibQdownloader.Threading.JobEventArgs<FileCopyJob>>(copyMan_JobStarting);
            copyMan.WorkCompleted += new EventHandler(copyMan_WorkCompleted);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            copyMan.Dispose();
        }

        void copyMan_WorkCompleted(object sender, EventArgs e)
        {
            viewModelSync.SaveAllM3uPlaylists();
            viewModelSync.Status = "";
            pnlFinished.Visibility = System.Windows.Visibility.Visible;
            pnlProgress.Visibility = System.Windows.Visibility.Collapsed;
            this.SizeToContent = System.Windows.SizeToContent.Height;
        }

        void copyMan_JobStarting(object sender, LibQdownloader.Threading.JobEventArgs<FileCopyJob> e)
        {
            Current++;
            viewModelCopy.From = e.Job.Source;
            viewModelCopy.To = e.Job.Destination;
            viewModelCopy.Size = e.Job.Size;
            viewModelSync.Status = "Copying " + System.IO.Path.GetFileNameWithoutExtension(e.Job.Source);
            viewModelCopy.Percentage = (int)(Common.CalculatePercent(Current, Total) + 0.5f); // Ceiling
        }

        private void sync_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (viewModelSync.HasCapacityExceeded)
            {
                MessageBox.Show(string.Format("Current selection exceeds drive capacity by {0}", Common.ToReadableSize(-viewModelSync.RemainingCapacity)));
            }
            else
            {
                List<FileCopyJob> jobs = new List<FileCopyJob>(viewModelSync.GetSelectedTracksUniqueAsFileCopyJobs());
                Total = jobs.Count;
                copyMan.Enqueue(jobs);
                pnlProgress.Visibility = System.Windows.Visibility.Visible;
                pnlSelection.Visibility = System.Windows.Visibility.Collapsed;
                this.SizeToContent = System.Windows.SizeToContent.Height;
            }
        }


        private void sync_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            if (string.IsNullOrWhiteSpace(viewModelSync.Path))
            {
                e.CanExecute = false;
            }
            else if(viewModelSync.SelectedTracksSize <= 0)
            {
                e.CanExecute = false;
            }
        }

        private void close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
