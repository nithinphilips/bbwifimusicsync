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

using System.IO;
using System.Xml.Serialization;
using libMusicSync.Model;

namespace WifiSyncServer.Model
{
    public class Subscription : Request, ISubscription
    {
        public string[] Playlists { get; set; }
 
        public override bool CheckValidate(out Response errorResponse)
        {
            if (!CheckDeviceId(out errorResponse) ||
              !CheckDeviceMediaRoot(out errorResponse))
            {
                return false;
            }

            DeviceMediaRoot = DeviceMediaRoot.Trim();
            if (!DeviceMediaRoot.EndsWith("/")) DeviceMediaRoot = DeviceMediaRoot + "/";
            return true;
        }

        public static Subscription Deserialize(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Subscription));
            using (Stream s = File.OpenRead(path))
            {
                return deserializer.Deserialize(s) as Subscription;
            }
        }

        public static void Serialize(Subscription obj, string path)
        {
            string dirName = System.IO.Path.GetDirectoryName(path);

            if ((dirName != null) && !Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            else if (File.Exists(path))
                File.Delete(path);

            XmlSerializer serializer = new XmlSerializer(typeof(Subscription));
            using (Stream s = File.OpenWrite(path))
            {
                serializer.Serialize(s, obj);
            }
        }

    }
}
