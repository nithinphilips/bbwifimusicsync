/**********************************************************************
 * WifiMusicSync
 * Copyright (C) 2011 Nithin Philips
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 **********************************************************************/

using System.Collections.Generic;
using System.Text;

namespace WifiSyncServer.Model
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
