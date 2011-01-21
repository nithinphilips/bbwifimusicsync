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
using System.Globalization;
using System.Linq;
using libMusicSync.Model;

namespace libMusicSync.Helpers
{
    public static class DiffHandler
    {
        
        //public static IEnumerable<SyncAction> Diff(IEnumerable<string> a, IEnumerable<string> b)
        //{
        //    List<SyncAction> result = new List<SyncAction>();

        //    System.Collections.Generic.HashSet<string> setA = new HashSet<string>(a);
        //    System.Collections.Generic.HashSet<string> setB = new HashSet<string>(b);

        //    // Let A = pc, B = phone

        //    foreach (var item in setA)
        //    {
        //        if (!setB.Contains(item))
        //        {
        //            result.Add(new SyncAction { Type = SyncType.Add, DeviceLocation = item });
        //        }
        //    }

        //    foreach (var item in setB)
        //    {
        //        if (!setA.Contains(item))
        //        {
        //            result.Add(new SyncAction { Type = SyncType.Remove, DeviceLocation = item });
        //        }
        //    }

        //    return result;
        //}


        /// <summary>
        /// Calculates the difference between two sets and returns a set of actions when performed on b, will make b identical to a.
        /// </summary>
        /// <remarks>
        /// The actions returned are ordered. Remove actions are first, then Add actions.
        /// In situations where the actions are applied to file systems, this will allow the operations to proceed to the end in situations where available storage space is limited.
        /// </remarks>
        /// <param name="a">The reference set. The actions are generated based on this set.</param>
        /// <param name="b">The target set. The actions are applicable to this set.</param>
        /// <returns>A set of actions when performed on b, will make b identical to a.</returns>
        public static IEnumerable<SyncAction> Diff(IEnumerable<string> a, IEnumerable<string> b)
        {
            if (a == null) throw new ArgumentNullException("a", "Parameter cannot be null");
            if (b == null) throw new ArgumentNullException("b", "Parameter cannot be null");

            StringComparer ignoreCaseComparer = StringComparer.Create(CultureInfo.CurrentCulture, true);

            ISet<string> setA = new HashSet<string>(a, ignoreCaseComparer);
            ISet<string> setB = new HashSet<string>(b, ignoreCaseComparer);

            setB.ExceptWith(a); // setB = Tracks to Remove
            setA.ExceptWith(b); // setA = Tracks to Add

            List<SyncAction> result = new List<SyncAction>(setA.Count + setB.Count);

            result.AddRange(setB.Select(s => new SyncAction { Type = SyncType.Remove, DeviceLocation = s }));
            result.AddRange(setA.Select(s => new SyncAction { Type = SyncType.Add,    DeviceLocation = s }));

            return result;
        }
    }
}
