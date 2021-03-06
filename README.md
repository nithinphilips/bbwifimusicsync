Wireless Music Sync for BlackBerry&reg;
=======================================
Wireless Music Sync for BlackBerry&reg; keeps the music on your BlackBerry&reg; devices
in sync with your Desktop music library wirelessly. It presently works with
iTunes on Windows.

Wireless Music Sync for BlackBerry&reg; has three components:

 1. **Server**: Runs in the background. Fulfills requests from clients.
 2. **Desktop Client**: For initial loading or large periodic transfers.
 3. **Client**: BlackBerry&reg; app. Lets you select Playlists, Albums or Artists to
    sync and Performs sync.

If you are a new user, see [Getting
Started](https://github.com/nithinphilips/bbwifimusicsync/blob/master/GettingStarted.mkd)
for detailed installation instructions.

###Features
 * Sync a Playlist, an Album or an Artist.
 * Fast, On-Demand sync.
 * Supports both OS 5 and 6.
 * Server uses minimal amount of memory while idle.
 * Two-Way syncing of playlists (for iTunes).
 * Open protocol and code, modify it to your heart's content.

###Planned Features
 * File based backend support.
 * Support for other popular music players.
 * Linux server.

Using WiFi Music Sync
---------------------
There are several ways to get started with Wireless Music Sync for
BlackBerry&reg;. The recommended way is to initially load music on to your
BlackBerry&reg; device using the **Desktop Client**.  You can then start the
Wifi Sync application on your phone and the synced playlists will be kept
updated. see [Getting
Started](https://github.com/nithinphilips/bbwifimusicsync/blob/master/GettingStarted.mkd)
for detailed installation instructions.

###Runtime Dependencies
To run the server application you need:

 * Windows XP or higher.
 * [.NET FX 4.0](http://msdn.microsoft.com/en-us/netframework/aa569263)
 * [iTunes](https://www.apple.com/itunes/) (Optional)

###Build Dependencies
To build Wireless Music Sync for BlackBerry&reg; you need:

 * [.NET Framework](http://msdn.microsoft.com/en-us/netframework/default) + Tools (>= 4.0)
 * [Kayak Server Framework](https://github.com/kayak/kayak) (source included)
 * [Afterthought](https://github.com/vc3/Afterthought) (source included)
 * [Log4net](http://logging.apache.org/log4net/) (>= 1.2.9.0) (unmodified binary included)
 * [iTunes](http://www.apple.com/itunes/) (>= 9.0)
 * *And* [Visual Studio.NET 2010](https://www.microsoft.com/express/Downloads/)+ (Optional) It is the easiest way to build.

Known Issues
------------
 * The BlackBerry app cannot sync playlists with special characters in the
   name, characters that cannot be represented in the BlackBerry file system
   (FAT32) file names. Playlists with Unicode characters in the name will
   usually fail to sync.

Get It
------
 * [Download the Latest Release](http://sourceforge.net/projects/bbwifimusicsync/files/)
 * [Fork it on GitHub](https://github.com/nithinphilips/bbwifimusicsync) or 
   [Get Source Code](http://sourceforge.net/projects/bbwifimusicsync/develop)
 * Install the BlackBerry Client OTA [OS 6](http://bbwifimusicsync.sourceforge.net/Web/6.0.0/WifiMusicSync.jad)
   or [OS 5](http://bbwifimusicsync.sourceforge.net/Web/5.0.0/WifiMusicSync.jad)

Credits
-------

 * [Steven M. Cohn](http://www.codeproject.com/script/Membership/View.aspx?mid=225718) for
   [USBManager](http://www.codeproject.com/KB/cs/UsbManager.aspx). 
   Also check out [iTuner](https://ituner.codeplex.com/).
 * [Twit88](http://twit88.com/) for [QRCodeLib](http://twit88.com/home/opensource/qrcode).
 * [Nir Dobovizki](http://www.nbdtech.com/) for 
   [WPF reflection control](http://www.nbdtech.com/Blog/archive/2007/11/21/WPF-Reflection-Control.aspx).
 * [Eric Daugherty](https://sourceforge.net/users/edaugherty) for 
   [iTunesExport](https://sourceforge.net/projects/itunesexport) (the old C# version).
 * [George Joseph](mailto:george.joseph@fairview5.com) for code used from 
   [KeePass for BlackBerry](http://f5bbutils.fairview5.com/keepassbb2/).
 * Icons based on
   [Faenza](https://tiheum.deviantart.com/art/Faenza-Icons-173323228),
   [Elementary](https://launchpad.net/elementaryicons) or
   [Ultimate-Gnome](https://code.google.com/p/ultimate-gnome/) icon themes.

 * Thanks to Brian Gold for testing the use of iTunes libraries stored on
   network shares.

License
-------

    Copyright (C) 2011 Nithin Philips <nithin@nithinphilips.com>
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

[Visit the Project Page at SourceForge](http://sourceforge.net/projects/bbwifimusicsync/)
<br />
<a href="http://sourceforge.net/" title="visit SourceForge.net">
    <img alt="SourceForge.net Logo" src="http://sourceforge.net/sflogo.php?group_id=402939&amp;type=13"/>
</a>

BlackBerry&reg; is a [registered trademark](http://us.blackberry.com/legal/trademarks.jsp) of Research in Motion.
