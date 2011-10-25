Wireless Music Sync for BlackBerry®
===================================
Wireless Music Sync for BlackBerry® keeps the music on your BlackBerry® devices
in sync with your Desktop music library wirelessly.

Release 0.3 (2011-10-25)
------------------------
This release brings improved stability and support for customizing the iTunes
library file path.

**NOTE:** The directory structure of the files installed by the installer has
changed. In order to avoid conflicts, you **MUST** manually uninstall any previous
versions before installing 0.3.

###Source Code

  + [@GitHub](https://github.com/nithinphilips/bbwifimusicsync/tree/fcbb4653eba18245b7e547d2a9f5614ba159c064)
  + or [@SourceForge](http://bbwifimusicsync.git.sourceforge.net/git/gitweb.cgi?p=bbwifimusicsync/bbwifimusicsync;a=tree;h=aff3a150bb2e89e8f02ef14b47c6128bd15dc89f;hb=fcbb4653eba18245b7e547d2a9f5614ba159c064)

###New Features

 + Ability to customize the iTunes library path.
   + Works with libraries on CIFS network shares as well (Thanks Brian.)
 + Simplified OTA installation via QRCodes.
   + The server now attempts to identify its NAT IP address and provides it to
     the user.
 + Added a context menu item to the Server application's tray icon to quickly
   access the webpage.
 + Additional documentation.

###Changes & Bug Fixes

 + BlackBerry Client is now more responsive while syncing.
 + Fixed a bug in the BlackBerry client that caused the playlist selections
   made while the list was filtered to be silently ignored.
 + Server's webpage now has a favicon.
 + Improved the appearance of the server's webpage.
 + Simplified build process using Rake and Albacore.
 + Ability to sync changes made on the phone to the PC has been temporarily
   disabled.

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

BlackBerry® is a [registered trademark](http://us.blackberry.com/legal/trademarks.jsp) of Research in Motion.
