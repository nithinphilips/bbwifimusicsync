using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiSyncDesktopClient.Threading
{
    public class FileCopyJob
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Size { get; set; }
    }
}
