using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WifiSyncDesktopClient.Model
{
    public class DirectorySizeCalculator
    {
        public static long CalculateSize(string path)
        {
            if (!Directory.Exists(path)) return 0;

            long totalSize = 0;
            //TODO: Handle Access denied exceptions
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                totalSize += fi.Length;
            }
            System.Diagnostics.Debug.WriteLine(totalSize);
            return totalSize;
        }
    }
}
