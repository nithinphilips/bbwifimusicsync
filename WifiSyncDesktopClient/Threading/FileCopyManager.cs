using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibQdownloader.Threading;
using log4net;

namespace WifiSyncDesktopClient.Threading
{
    class FileCopyManager : WorkManager<FileCopyJob, FileCopier>
    {
        private static readonly ILog log = LogManager.GetLogger("FileCopyManager");

        public FileCopyManager()
                : base(1, log, "FileCopier") { }

        // We implement a new event, so that we can update the ui with our progress
        public event EventHandler<FileCopyJobEventArgs> JobProgress;

        protected override bool DoWork(FileCopyJob job)
        {
            FileCopier worker = new FileCopier();
            base.AddActiveWorker(worker);
            worker.Work(job);
            base.RemoveActiveWorker(worker);
            return true;
        }

        void worker_JobProgress(object sender, FileCopyJobEventArgs e)
        {
            base.Post<FileCopyJobEventArgs>(JobProgress, e);
        }
    }
}
