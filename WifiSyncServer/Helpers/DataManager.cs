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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WifiSyncServer.Model;

namespace WifiSyncServer.Helpers
{
    public sealed class DataManager
    {
        public static string ApplicationStorageRoot = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MusicSync");

        public static string GetDeviceInfoStorageDir(string safeDeviceId)
        {
            Directory.CreateDirectory(safeDeviceId);
            return safeDeviceId;
        }

        public static string GetDevicePlaylistDir(PlaylistRequest request)
        {
            return GetDevicePlaylistDir(request.SafeDeviceId, request.SafePlaylistDevicePath);
        }

        public static string GetDevicePlaylistDir(string safeDeviceId, string playlistSafeName)
        {
            return Path.Combine(GetDeviceInfoStorageDir(safeDeviceId), playlistSafeName);
        }

        public static string GetSubscriptionPath(string safeDeviceId)
        {
            return Path.Combine(safeDeviceId, "Subscription.xml");
        }

        public static Subscription GetSubscription(string safeDeviceId)
        {
            string path = GetSubscriptionPath(safeDeviceId);
            return File.Exists(path) ? Subscription.Deserialize(path) : null;
        }

        public static void SaveSubscription(Subscription obj)
        {
            if(obj != null) Subscription.Serialize(obj, GetSubscriptionPath(obj.SafeDeviceId));
        }
    }
}
