using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WifiMusicSync
{
    class DiffHandler
    {
        public static IEnumerable<SyncAction> ReadDiff(string path)
        {
            List<SyncAction> result = new List<SyncAction>();
            // Read diff file
            Console.WriteLine("Loading diff...");
            using (StreamReader sr = File.OpenText(path))
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
                                result.Add(new SyncAction { Type = SyncType.Add, RemotePath = line.Substring(1) });
                            }
                            else if (line[0] == '-')
                            {
                                result.Add(new SyncAction { Type = SyncType.Remove, RemotePath = line.Substring(1) });
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
