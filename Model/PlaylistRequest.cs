using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiMusicSync.Helpers;

namespace WifiMusicSync.Model
{
    public class PlaylistRequest
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlaylistRequest).Name);


        public string DeviceId { get; set; }
        public string PlaylistDevicePath { get; set; }
        public string DeviceMediaRoot { get; set; }
        public string[] PlaylistData { get; set; }

        public SyncResponse CheckValidate()
        {
            SyncResponse errorResponse = null;
            errorResponse = Check(DeviceId, "DeviceId");
            if (errorResponse != null) return errorResponse;

            errorResponse = Check(PlaylistDevicePath, "PlaylistDevicePath");
            if (errorResponse != null) return errorResponse;

            errorResponse = Check(DeviceMediaRoot, "DeviceMediaRoot");
            if (errorResponse != null) return errorResponse;
            

            RemoveEmptyLinesFromPlaylistData();
            PlaylistDevicePath = PlaylistDevicePath.Trim();
            DeviceMediaRoot = DeviceMediaRoot.Trim();
            if (!DeviceMediaRoot.EndsWith("/")) DeviceMediaRoot = DeviceMediaRoot + "/";

            return null;
        }

        private SyncResponse Check(string parameter, string paramterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                log.Warn("Required parameter is missing or empty: " + paramterName);
                return new SyncResponse { ErrorMessage = "Required parameter is missing or empty: " + paramterName, Error = (int)SyncResponse.SyncResponseError.RequiredParameterMissing };
            }
            return null;
        }

        public void RemoveEmptyLinesFromPlaylistData()
        {
            if ((PlaylistData != null) && (PlaylistData.Length > 0))
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
            else
            {
                log.Debug("Playlist data is empty");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DeviceId: " + DeviceId);
            sb.AppendLine("PlaylistDevicePath: " + PlaylistDevicePath);
            sb.AppendLine("DeviceMediaRoot: " + DeviceMediaRoot);
            foreach (var item in PlaylistData)
            {
                sb.AppendLine(" > " + item);
            }
            return sb.ToString();
        }
    }
}
