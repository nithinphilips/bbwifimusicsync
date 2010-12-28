using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiMusicSync.Model
{
    public class PlaylistRequest
    {
        public string PlaylistDevicePath { get; set; }
        public string[] PlaylistData { get; set; }
    }
}
