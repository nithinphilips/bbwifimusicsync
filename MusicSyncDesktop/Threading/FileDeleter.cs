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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibQdownloader.Threading;

namespace WifiSyncDesktop.Threading
{
    class FileDeleter : FileWorker
    {
        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override bool Paused
        {
            get { return false; }
        }

        public override  void Resume()
        {
           
        }

        public override  void Dispose()
        {
           
        }

        public override void Work(FileOperation job)
        {
            File.Delete(job.Source);

            string dir = job.Source;
            while (!string.IsNullOrEmpty(dir = Path.GetDirectoryName(dir)))
            {
                if ((Directory.GetFiles(dir).Length == 0) && (Directory.GetDirectories(dir).Length == 0))
                    Directory.Delete(dir);
                else
                    break;
            }
        }
    }
}
