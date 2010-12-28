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

        public static IEnumerable<SyncAction> DiffA(string originalPath, string modifiedPath, string outputPath)
        {
            string parameters = string.Format("/createunifieddiff /origfile:{0} /modifiedfile:{1} /outfile:{2}", originalPath, modifiedPath, outputPath);
            ProcessStartInfo si = new ProcessStartInfo("TortoiseMerge", parameters);
            si.CreateNoWindow = true;
            si.WindowStyle = ProcessWindowStyle.Minimized;
            Process.Start(si).WaitForExit();


            List<SyncAction> result = new List<SyncAction>();
            // Read diff file
            using (StreamReader sr = File.OpenText(outputPath))
            {
                if (sr.Peek() > 0)
                {
                    // skip headers
                    sr.ReadLine();
                    sr.ReadLine();

                    while (sr.Peek() > 0)
                    {
                        string line = sr.ReadLine();
                        if (line.Length >= 1)
                        {
                            if (line[0] == '+')
                            {
                                result.Add(new SyncAction { Type = SyncType.Add, DeviceLocation = line.Substring(1) });
                            }
                            else if (line[0] == '-')
                            {
                                result.Add(new SyncAction { Type = SyncType.Remove, DeviceLocation = line.Substring(1) });
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
