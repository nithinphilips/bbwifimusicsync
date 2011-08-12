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
using System.IO;

namespace WifiSyncDesktop.Threading
{
    class FileCopier : FileWorker
    {

        public override bool Paused
        {
            get { return false; }
        }


        #region IPauseable Members

        public override void Pause()
        {
            
        }

        public override void Resume()
        {
            
        }

        #endregion

        public override void Work(FileOperation job)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(job.Destination));
            if(File.Exists(job.Destination))
            {
                // Check if the sizes are the same and skip copying if it is.
                FileInfo srcInfo = new FileInfo(job.Source);
                FileInfo destInfo = new FileInfo(job.Destination);

                if (srcInfo.Length == destInfo.Length) return;
            }

            File.Copy(job.Source, job.Destination, true);
        }

        #region IDisposable Members

        public override void Dispose()
        {
            
        }

        #endregion
    }
}
