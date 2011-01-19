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
using LibQdownloader.Threading;
using log4net;

namespace WifiSyncDesktop.Threading
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
