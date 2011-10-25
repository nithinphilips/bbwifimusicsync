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
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using Kayak.Framework;
using Kayak;
using libMusicSync.iTunesExport.Parser;
using log4net.Config;
using log4net;
using WifiSyncServer.Model;
using WifiSyncServer.Properties;

namespace WifiSyncServer
{
    public sealed class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("MusicSync.Server");

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

            XmlConfigurator.Configure(new FileInfo("server.log.config"));

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var server = new KayakServer(new System.Net.IPEndPoint(0, Settings.Default.Port));
            var behavior = new KayakFrameworkBehavior();
            behavior.JsonMapper.SetOutputConversion<int>((i, w) => w.Write(i.ToString()));

            var framework = server.UseFramework();

            Log.Info("Now: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            Log.Info("Wifi Sync Server listening on " + GetIP() + ":" + server.ListenEndPoint.Port);
            

            NotifyIcon notifyIcon = new NotifyIcon();

            MenuItem exitMenu = new MenuItem("Exit", (sender, e) =>
                                                         {
                                                             notifyIcon.Visible = false;
                                                             Application.Exit();
                                                         });

            MenuItem controlPanelMenu = new MenuItem("Show Control Panel", (sender, e) =>
                                                                           {
                                                                               Process.Start(GetAccessUrl());
                                                                           });

            notifyIcon.Icon = Resources.music_sync_server;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { controlPanelMenu, exitMenu });
            notifyIcon.Text = "Music Sync Server Running";
            notifyIcon.Visible = true;
            
            Application.EnableVisualStyles();
            Application.Run();
            
            // unsubscribe from server (close the listening socket)
            framework.Dispose();

            Log.Info("All this has happened before, and all this will happen again.");
        }

        static string GetAccessUrl()
        {
            return string.Format("http://{0}:{1}", GetIP(), Settings.Default.Port);
        }

        static string GetIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            if(ex == null)
                Log.Error("An unhandled exception: " + e.ExceptionObject);
            else
                Log.Error("An unhandled exception", ex);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log.Error("An unhandled exception", e.Exception);
        }

        
    }
}
