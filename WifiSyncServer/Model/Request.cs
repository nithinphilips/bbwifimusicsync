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
using System.Linq;
using System.Text;
using libMusicSync.Helpers;
using libMusicSync.Model;

namespace WifiSyncServer.Model
{
    public abstract class Request
    {
        /// <summary>
        /// Get or sets the root folder where media is stored on the device.
        /// </summary>
        public string DeviceMediaRoot { get; set; }

        /// <summary>
        /// Get or sets a unique id that represents the device. Allows the server to track subscriptions and support two-way syncing.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets a hash of the device ID that can used as key for storing data.
        /// </summary>
        public string SafeDeviceId
        {
            get { return Helper.GetSha1Hash(DeviceId ?? ""); }
        }

        /// <summary>
        /// Checks and validates the contents of the request.
        /// </summary>
        /// <param name="errorResponse">If a check failed, an error that can be sent to be client.</param>
        /// <returns>True, if the request is valid, otherwise false.</returns>
        public abstract bool CheckValidate(out Response errorResponse);

        protected bool CheckDeviceId(out Response errorResponse)
        {
            return CheckNullOrWhiteSpace(DeviceId, "DeviceId", out errorResponse);
        }

        protected bool CheckDeviceMediaRoot(out Response errorResponse)
        {
            return CheckNullOrWhiteSpace(DeviceMediaRoot, "DeviceMediaRoot", out errorResponse);
        }

        protected static bool CheckNullOrWhiteSpace(string parameter, string parameterName, out Response errorResponse)
        {
            errorResponse = null;
            if (string.IsNullOrWhiteSpace(parameter))
            {
                errorResponse = new SyncResponse { ErrorMessage = "Required parameter is missing or empty: " + parameterName, Error = (int)SyncResponseError.RequiredParameterMissing };
                return false;
            }

            return true;
        }
    }
}
