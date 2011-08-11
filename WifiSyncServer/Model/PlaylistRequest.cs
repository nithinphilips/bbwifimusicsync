/**********************************************************************
 * WifiMusicSync
 * Copyright (C) 2011 Nithin Philips <nithin@nithinphilips.com>
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
using System.Linq;
using System.Text;
using libMusicSync.Helpers;

namespace WifiSyncServer.Model
{
    public class PlaylistRequest : Request
    {
        public string PlaylistDevicePath { get; set; }
        public string[] PlaylistData { get; set; }

        /// <summary>
        /// Gets a hash of the PlaylistDevicePath that can used as key for storing data.
        /// </summary>
        public string SafePlaylistDevicePath
        {
            get { return Helper.GetSha1Hash(PlaylistDevicePath ?? ""); }
        }

        public override bool CheckValidate(out Response errorResponse)
        {

            if(!CheckDeviceId(out errorResponse) ||
               !CheckDeviceMediaRoot(out errorResponse) ||
               !CheckNullOrWhiteSpace(PlaylistDevicePath, "PlaylistDevicePath", out errorResponse))
            {
                return false;
            }

            RemoveEmptyLinesFromPlaylistData();
            PlaylistDevicePath = PlaylistDevicePath.Trim();
            DeviceMediaRoot = DeviceMediaRoot.Trim();
            if (!DeviceMediaRoot.EndsWith("/")) DeviceMediaRoot = DeviceMediaRoot + "/";

            return true;
        }

        public void RemoveEmptyLinesFromPlaylistData()
        {
            if ((PlaylistData != null) && (PlaylistData.Length > 0))
            {
                List<string> result = new List<string>(PlaylistData.Length);
                result.AddRange(from item in PlaylistData where !string.IsNullOrWhiteSpace(item) select item.Trim());
                PlaylistData = result.ToArray();
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
                sb.AppendLine(">>> " + item);
            }
            return sb.ToString();
        }
    }
}
