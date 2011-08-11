using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibQdownloader.Threading;

namespace WifiSyncDesktop.Threading
{
    public abstract class FileWorker:  IDisposable, IPauseable
    {
        public abstract void Dispose();

        public abstract void Pause();

        public abstract bool Paused { get; }

        public abstract void Resume();

        public abstract void Work(FileOperation job);
    }
}
