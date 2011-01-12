/**********************************************************************
 * WifiMusicSync
 * Copyright (C) 2011 Nithin Philips
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
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using WifiMusicSync.Model;

namespace WifiMusicSync.Helpers
{
    class DiffHandler
    {
        
        public static IEnumerable<SyncAction> Diff(IEnumerable<string> A, IEnumerable<string> B)
        {
            List<SyncAction> result = new List<SyncAction>();

            System.Collections.Generic.HashSet<string> setA = new HashSet<string>(A);
            System.Collections.Generic.HashSet<string> setB = new HashSet<string>(B);

            // Let A = pc, B = phone

            foreach (var item in setA)
            {
                if (!setB.Contains(item))
                {
                    Debug.Assert(item.StartsWith("file"));
                    result.Add(new SyncAction { Type = SyncType.Add, DeviceLocation = item });
                }
            }

            foreach (var item in setB)
            {
                if (!setA.Contains(item))
                {
                    Debug.Assert(item.StartsWith("file"));
                    result.Add(new SyncAction { Type = SyncType.Remove, DeviceLocation = item });
                }
            }

            return result;
        }
    }
}
