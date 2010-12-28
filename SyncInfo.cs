using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiMusicSync
{
    public class SyncInfo
    {
        public SyncInfo()
        {
            ServerId = "WiFiMusicSync 1.0";
        }

        public string ServerId { get; set; }
        public string PlaylistPath { get; set; }
        public SyncAction[] Actions { get; set; }
    }
}
