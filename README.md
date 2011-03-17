BB Wifi Music Sync
==================

A Client-Server program to keep the music on your Blackberry 
(and potentially other devices with WiFi capability) updated.

**Note:** This project is in no way associated with [RIM](http://www.rim.com/).

Why?
----
Well, Blackberry Desktop already implements a sweet music sync
option. Why reinvent the proverbial wheel, you may ask. Well, 
the answer is due to the way the BB Desktop sync works,
if you have a very large library (like I do), the program 
will not do much beyond making your Blackberry grind to a halt.
I stared at the busy cursor for far too long and heard no music!
Thus Wifi Music Sync was born.

Wifi Music Sync only pushes out only the songs and playlists
_you want_, while trying to do as much work as it can on the 
server side, leaving your battery free to...um..make phone calls.

Even when you only want to sync manually, the Blackberry 
Desktop Software takes long enough to load, you'll have forgotten
why you connected your phone to the PC in the first place. So, 
with the Music Sync Desktop program, which will load rather 
quickly you can easily keep your music updated even 
without Wifi.

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
 * .NET FX 4.0 (Needs to be verfied).
 * iTunes.

###Build Dependencies

To build Wifi Music Sync you need:

 * [.NET Framework](http://msdn.microsoft.com/en-us/netframework/default) + Tools (>= 4.0)
 * [Kayak Server Framework](https://github.com/kayak/kayak) (source included)
 * [PostSharp](http://www.sharpcrafters.com/postsharp) (>= 2.0) _(Get the free Community Edition)_
 * [Log4net](http://logging.apache.org/log4net/) (>= 1.2.9.0) (unmodified binary included)
 * [iTunes](http://www.apple.com/itunes/) (>= 9.0)

AND

* [VS.NET 2010](https://www.microsoft.com/express/Downloads/)+ (Optional) It is the easiest way to build.

Get It
------

 * [Download the Latest Release](http://sourceforge.net/projects/bbwifimusicsync/files/)
 * [Get Source Code](http://sourceforge.net/projects/bbwifimusicsync/develop)

Links
-----

 * [SourceForge Project Page](http://sourceforge.net/projects/bbwifimusicsync/)

Future Plans
------------
Rewrite the server in Vala (or use Mono) for GNOME. We can add support
for Banshee by interacting with the SQLite database. The problems with 
locking etc. of the database need to be investigated. A plugin is out of
scope because we need to be always on.

-------------------------------------------------------------------
_$Id$_
