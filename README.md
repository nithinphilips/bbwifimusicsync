<!--!# { "pagewidth": "850px" } -->
BB Wifi Music Sync
==================

BBWifiMusicSync is a Client-Server program to keep the music updated on
your Blackberry, and potentially other, devices with WiFi capability.

Wifi Music Sync only pushes out only the songs and playlists
_you want_, while trying to do as much work as it can on the 
server side.

###Features

 * Sync a Playlist, an Album or an Artist.
 * Fast, On-Demand sync.
 * Server uses minimal amount of memory while idle.
 * Two-Way sync.
   * Usually iTunes must be running to make changes to the playlists
     on the PC, but BBWifiMusicSync will remember all the changes and apply
     it whenever iTunes is run by the user.
 * Open protocol and code, modify it to your heart's content.
 * Potential to add support for any number of media players and even
   simple file based access.

Using WiFi Music Sync
---------------------
There are several ways to get started with Music Sync. The recommended
way is to initially load music on to your Blackberry device using the
Desktop Sync application. Then you can start the Wifi Sync application
on your phone and the necessary server settings and the synced playlists
will be automatically imported. You'll then have to simply choose to sync
occasionally to keep the music on your Blackberry updated.

###Runtime Dependencies

To run the server application you need:

 * Windows XP or higher.
 * .NET FX 4.0.
 * iTunes.

###Dependencies

To build Wifi Music Sync you need:

 * [.NET Framework](http://msdn.microsoft.com/en-us/netframework/default) + Tools (>= 4.0)
 * [Kayak Server Framework](https://github.com/kayak/kayak) (source included)
 * [Afterthought](https://github.com/vc3/Afterthought) (source included)
 * [Log4net](http://logging.apache.org/log4net/) (>= 1.2.9.0) (unmodified binary included)
 * [iTunes](http://www.apple.com/itunes/) (>= 9.0)

And

* [VS.NET 2010](https://www.microsoft.com/express/Downloads/)+ (Optional) It is the easiest way to build.

Get It
------

 * [Download the Latest Release](http://sourceforge.net/projects/bbwifimusicsync/files/)
 * [Get Source Code](http://sourceforge.net/projects/bbwifimusicsync/develop)
 * [Install Blackberry Client OTA](Web/6.0.0/WifiMusicSync.jad)

Links
-----

 * [SourceForge Project Page](http://sourceforge.net/projects/bbwifimusicsync/)

-------------------------------------------------------------------
<a style="float: right;" href="http://sourceforge.net/" title="visit SourceForge.net">
    <img alt="SourceForge.net Logo" src="http://sourceforge.net/sflogo.php?group_id=402939&amp;type=13"/>
</a>
Page generated from [README.md](README.md).<br />
_$Id$_
