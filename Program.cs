﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using iTunesLib;
using System.Security.Cryptography;
using Kayak.Framework;
using Kayak;
using WifiMusicSync.Properties;
using WifiMusicSync.iTunes;
using log4net.Config;
using log4net;

namespace WifiMusicSync
{
    public sealed class Program
    {
        private static readonly ILog log = LogManager.GetLogger("WifiMusicSync");

        static void Main(string[] args)
        {
            /* NOTE: This info is out-of-date
             * 
             * Basic Operation:
             * 1. Listen on port 8000 for http connection
             * 
             * For each playlist:
             * 2. PHONE posts "xyz.m3u" 
             * 3. SERVER Infer the name of the playlist from the file name.
             * 4. SERVER look for the playlist in iTunes and generate an m3u file.
             * 5. SERVER sort and then diff Phone playlist and iTunes generated playlist.
             * 6. SERVER respond with Json to the phone instructing it what to do:
             *      {
             *          "Server" : "wirelessSyncServer",
             *          "Playlist": "http://192.168.0.104:8000/xyz.m3u" ,
             *          "Get" : 
             *              [ 
             *                  { 
             *                      "RemoteId" : "http://192.168.0.104:8000/songs/iTunesID",
             *                      "LocalPath" : "file://....."
             *                  },
             *                  { 
             *                      "RemoteId" : "http://192.168.0.104:8000/songs/iTunesID",
             *                      "LocalPath" : "file://....."
             *                  }
             *              ],
             *              
             *          "Delete" : 
             *              [ 
             *                  { 
             *                      "LocalPath" : "file://....."
             *                  },
             *                  { 
             *                      "LocalPath" : "file://....."
             *                  }
             *              ]
             *      }
             * 7. PHONE parses JSON
             * 8. PHONE Downloads remote songs to phone.
             * 9. PHONE Deletes removed songs.
             * 10. PHONE Dowloads new playlist and replaces the old one.
             * 11. Hopefully music player will auto update.
             */

            XmlConfigurator.Configure(new FileInfo("log.config"));

            var server = new KayakServer(new System.Net.IPEndPoint(0, Settings.Default.Port));
            var behavior = new KayakFrameworkBehavior();
            behavior.JsonMapper.SetOutputConversion<int>((i, w) => w.Write(i.ToString()));

            var framework = server.UseFramework();

            log.Info("Now: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            log.Info("WifiMusicSync listening on " + server.ListenEndPoint);
            Console.ReadLine();

            // unsubscribe from server (close the listening socket)
            framework.Dispose();
        }

        
    }
}
