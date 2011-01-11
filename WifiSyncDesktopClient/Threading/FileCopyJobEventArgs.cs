using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiSyncDesktopClient.Threading
{
    public sealed class FileCopyJobEventArgs : EventArgs
    {
        public FileCopyJobEventArgs(int progress, FileCopyJob job)
        {
                this.Progress = progress;
                this.Job = job;
        }

        public int Progress { get; private set; }
        public FileCopyJob Job { get; private set; }
    }
}
