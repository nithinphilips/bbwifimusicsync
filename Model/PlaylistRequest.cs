using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiMusicSync.Model
{
    public class PlaylistRequest
    {
        public string DeviceId { get; set; }
        public string PlaylistDevicePath { get; set; }
        public string DeviceMediaRoot { get; set; }
        public string[] PlaylistData { get; set; }

        public void CheckValidate()
        {
            RemoveEmptyLinesFromPlaylistData();
            PlaylistDevicePath = PlaylistDevicePath.Trim();
            DeviceMediaRoot = DeviceMediaRoot.Trim();
            if (!DeviceMediaRoot.EndsWith("/")) DeviceMediaRoot = DeviceMediaRoot + "/";
        }

        public void RemoveEmptyLinesFromPlaylistData()
        {
            List<string> result = new List<string>(PlaylistData.Length);
            foreach (var item in PlaylistData)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    result.Add(item.Trim());
                }
            }
            PlaylistData = result.ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PlaylistDevicePath: " + PlaylistDevicePath);
            sb.AppendLine("DeviceMediaRoot: " + DeviceMediaRoot);
            foreach (var item in PlaylistData)
            {
                sb.AppendLine("    " + item);
            }
            return sb.ToString();
        }
    }
}
