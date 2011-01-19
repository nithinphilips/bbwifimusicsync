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

using System.Text;
using libMusicSync.Model;

namespace WifiSyncServer.Model
{
    public class SyncResponse : Response
    {
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
                sb.AppendFormat(" < {0}\n", item);
            }
            return sb.ToString();
        }
    }
}
