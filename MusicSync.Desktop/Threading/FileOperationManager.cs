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
    class FileOperationManager : WorkManager<FileOperation, FileWorker>
    {
        private static readonly ILog log = LogManager.GetLogger("FileOperationManager");

        public FileOperationManager()
            : base(1, log, "FileWorker") { }

        // We implement a new event, so that we can update the ui with our progress
        public event EventHandler<FileOperationEventArgs> JobProgress;

        protected override bool DoWork(FileOperation job)
        {
            bool result = false;

            if (job.OperationType == FileOperationType.Copy)
            {
                FileCopier worker = new FileCopier();
                base.AddActiveWorker(worker);
                try
                {
                    worker.Work(job);
                    result = true;
                }catch(Exception ex)
                {
                    log.Error("Job failed: " + job, ex);
                }
                base.RemoveActiveWorker(worker);
            }else if(job.OperationType == FileOperationType.Delete)
            {
                FileDeleter worker = new FileDeleter();
                base.AddActiveWorker(worker);
                try
                {
                    worker.Work(job);
                    result = true;
                }
                catch (Exception ex)
                {
                    log.Error("Job failed: " + job, ex);
                }
                base.RemoveActiveWorker(worker);
            }

            return result;
        }

        void worker_JobProgress(object sender, FileOperationEventArgs e)
        {
            base.Post<FileOperationEventArgs>(JobProgress, e);
        }
    }
}
