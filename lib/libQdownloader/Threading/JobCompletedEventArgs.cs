using System;
using System.Collections.Generic;
using System.Text;

namespace LibQdownloader.Threading
{
    public class JobEventArgs<T> : EventArgs
    {
        T job;

        public T Job
        {
            get { return job; }
        }

        public JobEventArgs(T job)
        {
            this.job = job;
        }
    }
}
