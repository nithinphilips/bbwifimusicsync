using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WifiMusicSync
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
