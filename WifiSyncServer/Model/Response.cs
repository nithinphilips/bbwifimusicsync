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

namespace WifiSyncServer.Model
{
    public abstract class Response
    {
        protected Response()
        {
            ServerId = "WiFiMusicSync 1.0";

            Error = 0;
            ErrorMessage = null;
        }

        public int Error { get; set; }
        public string ErrorMessage { get; set; }
        public string ServerId { get; set; }

        public enum SyncResponseError
        {
            None = 0,
            RequiredParameterMissing = 10,
            PlaylistNotFound = 100
        }
    }
}
