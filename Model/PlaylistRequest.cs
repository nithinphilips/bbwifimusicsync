using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiMusicSync.Model
{
    public class PlaylistRequest
    {
        public string PlaylistDevicePath { get; set; }
        public string DeviceMediaRoot { get; set; }
        public string[] PlaylistData { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(PlaylistDevicePath);
            sb.AppendLine(DeviceMediaRoot);
            foreach (var item in PlaylistData)
            {
                sb.AppendLine("\t" + item);
            }
            return sb.ToString();
        }
    }
}
