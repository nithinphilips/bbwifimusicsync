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
        public static string ApplicationStorageRoot =  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Music Sync", "Sync Data");

        /// <summary>
        /// Gets the path to the directory where the data for a device can be stored.
        /// </summary>
        /// <param name="safeDeviceId">The safe ID of the device.</param>
        /// <returns>The path to the directory where the data for a device can be stored.</returns>
        public static string GetDeviceStorageDir(string safeDeviceId)
        {
            string path = Path.Combine(ApplicationStorageRoot, safeDeviceId);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets the path to the reference version of the playlist at playlistPath.
        /// </summary>
        /// <param name="playlistPath">The path of the playlist.</param>
        /// <returns>The path to the reference version of the playlist at playlistPath.</returns>
        public static string GetReferencePlaylistPath(string playlistPath)
        {
            return playlistPath + ".ref";
        }

        /// <summary>
        /// Gets the path to the file containing the list of pending changes.
        /// </summary>
        /// <param name="playlistPath">The path of the playlist.</param>
        /// <returns>The path to the file containing the list of pending changes.</returns>
        public static string GetChangeSetCollectionPath(string playlistPath)
        {
            return playlistPath + ".changes";
        }

        /// <summary>
        /// Gets the path to a particular playlist.
        /// </summary>
        public static string GetDevicePlaylistPath(PlaylistRequest request)
        {
            return GetDevicePlaylistPath(request.SafeDeviceId, request.SafePlaylistDevicePath);
        }

        public static string GetDevicePlaylistPath(string safeDeviceId, string playlistSafeName)
        {
            return Path.Combine(GetDeviceStorageDir(safeDeviceId), playlistSafeName);
        }

        /// <summary>
        /// Gets the path to the file where subscription information is stored.
        /// </summary>
        /// <param name="safeDeviceId">The safe ID of the device.</param>
        /// <returns>The path to the file where subscription information is stored.</returns>
        public static string GetSubscriptionPath(string safeDeviceId)
        {
            return Path.Combine(ApplicationStorageRoot, safeDeviceId, "Subscription.xml");
        }

        /// <summary>
        /// Get the subscription for the device with safe ID: safeDeviceId.
        /// </summary>
        /// <param name="safeDeviceId">The safe ID of the device.</param>
        /// <returns>The Subscription object, if it exists, otherwise null.</returns>
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
