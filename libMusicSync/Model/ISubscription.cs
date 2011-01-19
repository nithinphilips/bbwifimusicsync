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

namespace libMusicSync.Model
{
    public interface ISubscription
    {
        string[] Playlists { get; set; }

        /// <summary>
        /// Get or sets the root folder where media is stored on the device.
        /// </summary>
        string DeviceMediaRoot { get; set; }

        /// <summary>
        /// Get or sets a unique id that represents the device. Allows the server to track subscriptions and support two-way syncing.
        /// </summary>
        string DeviceId { get; set; }

        /// <summary>
        /// Gets a hash of the device ID that can used as key for storing data.
        /// </summary>
        string SafeDeviceId { get; }
    }
}
