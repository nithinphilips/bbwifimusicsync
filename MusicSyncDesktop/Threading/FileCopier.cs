using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibQdownloader.Threading;
using System.Threading;
using System.IO;

namespace WifiSyncDesktopClient.Threading
{
    class FileCopier : IDisposable, IPauseable
    {

        public bool Paused
        {
            get { return false; }
        }


        #region IPauseable Members

        public void Pause()
        {
            
        }

        public void Resume()
        {
            
        }

        #endregion

        public void Work(FileCopyJob job)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(job.Destination));
            File.Copy(job.Source, job.Destination, true);
        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
