using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiMusicSync
{
    public class SyncAction
    {
        public SyncType Type { get; set; }
        public string RemotePath { get; set; }
        public string LocalPath { get; set; }
        public string Id { get; set; }
    }
}
