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
            if(cachedSizes.ContainsKey(root))
            {
                return cachedSizes[root];
            }
            else
            {
                long size = GetDirectorySizeRecursive(root);
                FileSystemWatcher watcher = new FileSystemWatcher(root);
                watcher.IncludeSubdirectories = true;
                watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                watcher.EnableRaisingEvents = true;
                
                watchers.Add(root, watcher);
                cachedSizes.Add(root, size);
                return size;
            }
        }

        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            foreach (string key in cachedSizes.Keys)
            {
                if(e.FullPath.StartsWith(key))
                {
                    // invalidate cache
                    cachedSizes.Remove(key);
                    watchers[key].Dispose();
                    watchers.Remove(key);
                }
            }
        }

        static long GetDirectorySizeRecursive(string path)
        {
            long size = 0;
            string[] allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                FileInfo f = new FileInfo(allFiles[i]);
                size += f.Length;
            }
            return size;
        }
    }
}
