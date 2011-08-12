using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibQdownloader.Threading
{
    public class ConcreteJob
    {
        public ConcreteJob(string name) {
            this.name = name;
        }

        string name;

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public override string ToString() {
            return name;
        }
    }

    public class ConcreteWorker : IDisposable, IPauseable
    {
        volatile bool cancel;
        bool paused;

        public bool Paused {
            get { return paused; }
        }

        // We use a ManualResetEvent to pause and resume the work
        // normally, the handle is signaled, i.e. open, when paused, it will
        // be set to unsignaled or closed, blocking the work
        EventWaitHandle pauseHandle = new ManualResetEvent(true); // unpaused

        public event EventHandler<ConcreteJobProgressEventArgs> JobProgress;

        #region IPauseable Members

        public void Pause() {
            this.paused = true;
            pauseHandle.Reset();
        }

        public void Resume() {
            this.paused = false;
            pauseHandle.Set();
        }

        #endregion

        public void Work(ConcreteJob job) {

            for (int i = 1; i <= 100; i++) {
                pauseHandle.WaitOne(); // if paused, wait here
                if (cancel) return;
                if (JobProgress != null) {
                    JobProgress(this, new ConcreteJobProgressEventArgs(i, job));
                }
                Thread.Sleep(100);
            }
        }

        #region IDisposable Members

        public void Dispose() {
            this.cancel = true;
        }

        #endregion
    }

    public class ConcreteWorkManager : WorkManager<ConcreteJob, ConcreteWorker>
    {
        public ConcreteWorkManager()
            : base(3) { }

        // We implement a new event, so that we can update the ui with our progress
        public event EventHandler<ConcreteJobProgressEventArgs> JobProgress;

        protected override void DoWork(ConcreteJob job) {
            ConcreteWorker worker = new ConcreteWorker();
            worker.JobProgress += new EventHandler<ConcreteJobProgressEventArgs>(worker_JobProgress);
            base.AddActiveWorker(worker);
            worker.Work(job);
            base.RemoveActiveWorker(worker);
        }

        void worker_JobProgress(object sender, ConcreteJobProgressEventArgs e) {
            base.Post<ConcreteJobProgressEventArgs>(JobProgress, e);
        }
    }

    public sealed class ConcreteJobProgressEventArgs : EventArgs
	{
        public ConcreteJobProgressEventArgs(int progress, ConcreteJob job) {
            this.progress = progress;
            this.job = job;
        }

        int progress;

        public int Progress {
            get { return progress; }
        }
        ConcreteJob job;

        public ConcreteJob Job {
            get { return job; }
        }
	}
}
