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

using System.Collections.Generic;
using System.Diagnostics;
using WifiSyncServer.Model;

namespace WifiSyncServer.Helpers
{
    class DiffHandler
    {
        
        public static IEnumerable<SyncAction> Diff(IEnumerable<string> a, IEnumerable<string> b)
        {
            List<SyncAction> result = new List<SyncAction>();

            System.Collections.Generic.HashSet<string> setA = new HashSet<string>(a);
            System.Collections.Generic.HashSet<string> setB = new HashSet<string>(b);

            // Let A = pc, B = phone

            foreach (var item in setA)
            {
                if (!setB.Contains(item))
                {
                    result.Add(new SyncAction { Type = SyncType.Add, DeviceLocation = item });
                }
            }

            foreach (var item in setB)
            {
                if (!setA.Contains(item))
                {
                    result.Add(new SyncAction { Type = SyncType.Remove, DeviceLocation = item });
                }
            }

            return result;
        }
    }
}
