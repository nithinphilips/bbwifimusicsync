using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiMusicSync.Model
{
    public class SyncResponse
    {
        public SyncResponse()
        {
            ServerId = "WiFiMusicSync 1.0";

            Error = 0;
            ErrorMessage = null;
        }

        public int Error { get; set; }
        public string ErrorMessage { get; set; }
        public string ServerId { get; set; }
        public string PlaylistServerPath { get; set; }
        public string PlaylistDevicePath { get; set; }
        public SyncAction[] Actions { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Error != 0)
            {
                sb.AppendLine("Error: " + Error);
                sb.AppendLine("ErrorMessage: " + ErrorMessage);
            }

            sb.AppendLine("ServerId: " + ServerId);
            sb.AppendLine("PlaylistServerPath: " + PlaylistServerPath);
            sb.AppendLine("PlaylistDevicePath: " + PlaylistDevicePath);
            sb.AppendLine("Actions: ");
            foreach (var item in Actions)
            {
                sb.AppendFormat("    {0}\n", item);
            }
            return sb.ToString();
        }
    }
}
