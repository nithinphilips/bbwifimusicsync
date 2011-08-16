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
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using iTuner;
using WifiSyncDesktop.Helpers;
using WifiSyncDesktop.Model;
using LibQdownloader.Utilities;
using WifiSyncDesktop.Threading;
using WifiSyncDesktop.Properties;

namespace WifiSyncDesktop.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SyncSettings viewModelSync = new SyncSettings();
        CopyProgressModel viewModelCopy = new CopyProgressModel();
        private FileOperationManager _operationMan;

        public int Total { get; set; }
        public int Current { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Current = 0;

            _operationMan = new FileOperationManager();

            Parallel.Invoke(() =>
            {
                viewModelSync.LoadPlaylists();
                viewModelSync.LoadDrives();
            });
            

            this.DataContext = viewModelSync;
            this.pnlProgress.DataContext = viewModelCopy;

            _operationMan.JobStarting += new EventHandler<LibQdownloader.Threading.JobEventArgs<FileOperation>>(copyMan_JobStarting);
            _operationMan.WorkCompleted += new EventHandler(copyMan_WorkCompleted);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        #region System Menu Hooks

        // Define the Win32 API methods we are going to use
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        // Define our Constants we will use
        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 MF_SEPARATOR = 0x800;
        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 MF_STRING = 0x0;

        // The constants we'll use to identify our custom system menu items
        //public const Int32 _SettingsSysMenuID = 1000;
        public const Int32 _AboutSysMenuID = 1001;

        /// <summary>
        /// This is the Win32 Interop Handle for this Window
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return new System.Windows.Interop.WindowInteropHelper(this).Handle;
            }
        }

        private void HookSystemMenu()
        {
            // Get the Handle for the Forms System Menu
            IntPtr systemMenuHandle = GetSystemMenu(this.Handle, false);

            // Create our new System Menu items just before the Close menu item
            InsertMenu(systemMenuHandle, 5, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty); // <-- Add a menu seperator
            InsertMenu(systemMenuHandle, 6, MF_BYPOSITION, _AboutSysMenuID, "About...");
            
            

            // Attach our WndProc handler to this Window
            HwndSource source = HwndSource.FromHwnd(this.Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Check if a System Command has been executed
            if (msg == WM_SYSCOMMAND)
            {
                // Execute the appropriate code for the System Menu item that was clicked
                switch (wParam.ToInt32())
                {
                    case _AboutSysMenuID:
                        AboutWindow about = new AboutWindow();
                        about.Owner = this;
                        about.ShowDialog();
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        #endregion 

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _operationMan.Dispose();
        }

        void copyMan_WorkCompleted(object sender, EventArgs e)
        {
            viewModelSync.WritePlaylistFiles();
            viewModelSync.Status = "";
            pnlFinished.Visibility = Visibility.Visible;
            pnlProgress.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.Height;
        }

        void copyMan_JobStarting(object sender, LibQdownloader.Threading.JobEventArgs<FileOperation> e)
        {
            Current++;
            viewModelCopy.From = e.Job.Source;
            viewModelCopy.To = e.Job.Destination;
            viewModelCopy.Size = e.Job.Size;
            if(e.Job.OperationType == FileOperationType.Copy)
                viewModelSync.Status = "Copying " + System.IO.Path.GetFileNameWithoutExtension(e.Job.Source);
            else
                viewModelSync.Status = "Deleting " + System.IO.Path.GetFileNameWithoutExtension(e.Job.Source);

            viewModelCopy.Percentage = (int)(Common.CalculatePercent(Current, Total) + 0.5f); // Ceiling
        }

        private void sync_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (viewModelSync.HasCapacityExceeded)
            {
                MessageBox.Show(string.Format("Current selection exceeds drive capacity by {0}. Continue?", Common.ToReadableSize(-viewModelSync.RemainingCapacity)));
                //return;
            }

            Settings.Default.LastPath = viewModelSync.SyncPath;
            Settings.Default.Save();


            var jobs = viewModelSync.GetFileOperations().ToList();

            Total = jobs.Count;

            // We are reversing the operations, because in edge conditions where the available space is very limited, the 
            // track that we are deleting will free up the necessary amount of space and allow for successful completion.
            jobs.Reverse();

            _operationMan.Enqueue(jobs);
            pnlProgress.Visibility = Visibility.Visible;
            pnlSelection.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.Height;
        }


        private void sync_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(viewModelSync.SyncPath); // && viewModelSync.GetFileOperations().Any();
        }

        private void close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HookSystemMenu();
        }

    }
}
