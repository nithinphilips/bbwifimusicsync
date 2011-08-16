using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WifiSyncDesktop.Helpers
{
    public class DirectorySizeCalculator
    {
        static Dictionary<string, long> cachedSizes = new Dictionary<string, long>();
        static Dictionary<string, FileSystemWatcher> watchers = new Dictionary<string, FileSystemWatcher>();

        public static long GetDirectorySize(string root)
        {
            if (cachedSizes.ContainsKey(root))
            {
                return cachedSizes[root];
            }
            else
            {
                if (!Directory.Exists(root))
                {
                    return 0;
                }
                long size = GetDirectorySizeRecursive(root);
                cachedSizes.Add(root, size);
                return size;
            }
        }

        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var keys = (from k in cachedSizes.Keys
                       where k.StartsWith(e.FullPath)
                       select k).ToList();

            foreach (var key in keys)
            {
                cachedSizes.Remove(key);
                watchers[key].Dispose();
                watchers.Remove(key);
            }
    }

        static long GetDirectorySizeRecursive(string path)
        {
            string[] allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return allFiles.Select(t => new FileInfo(t)).Select(f => f.Length).Sum();
        }
    }
}
