using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiMusicSync.Model
{
    public class SyncAction
    {
        public SyncType Type { get; set; }
        public string DeviceLocation { get; set; }
        public string TrackPath { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Type, DeviceLocation);
        }
    }
}
