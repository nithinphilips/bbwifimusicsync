using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WifiMusicSync.Wired
{
    /// <summary>
    /// Performs Wired Synchronization
    /// </summary>
    public class Synchronizer
    {
        public string BlackberryPathToPcPath(string bbPath)
        {
            return bbPath.Replace("file:///SDCard/", "M:/");
        }

        public void Sync(SyncInfo info)
        {
            foreach (var item in info.Actions)
            {
                switch (item.Type)
                {
                    case SyncType.Add:
                        //AddToDevice(item.ServerLocation, item.DeviceLocation);
                        break;
                    case SyncType.Remove:
                        RemoveFromDevice(item.DeviceLocation);
                        break;
                    default:
                        break;
                }
            }


        }

        public void RemoveFromDevice(string devicePath)
        {
            File.Delete(BlackberryPathToPcPath(devicePath));
        }

        public void AddToDevice(string localPath, string devicePath)
        {
            File.Copy(localPath, BlackberryPathToPcPath(devicePath));
        }
    }
}
