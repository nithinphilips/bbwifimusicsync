using System;
using System.Collections.Generic;
using System.Text;

namespace LibQdownloader.Threading
{
    public interface IPauseable
    {

        /// <summary>
        /// Gets the status of the object. True, if work is paused, otherwise false.
        /// </summary>
        bool Paused { get; }

        /// <summary>
        /// Pauses work.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes work.
        /// </summary>
        void Resume();
    }

    public interface ICancelable : IPauseable
    {
        void Cancel();
    }
}
