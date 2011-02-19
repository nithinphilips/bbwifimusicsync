Wifi Music Sync
===============

A Client-Server program to keep the music on your Blackberry 
(and potentially other devices with Wifi) updated.

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

Runtime Dependencies
--------------------
To run the server you need:
 * .NET FX 4.0 (Needs to be verfied)
 * iTunes

Build Dependencies
------------------
To build the solution you need:
 * .NET Fx + Tools (>= 4.0)
 * PostSharp (>= 2.0) <http://www.sharpcrafters.com/postsharp>
    -> Get the free Community Edition (for the Desktop client)
 * Kayak Server Fx (source included)
 * Log4net (>= 1.2.9.0) (binary included)
 * iTunes (>= 9.0) <http://www.apple.com/itunes/>
 
 * VS.NET 2010+ is the easiest way to build.

Future Plans
------------
Rewrite the server in Vala for GNOME. This kind of application is
needed on the linux desktop and it'll be an opportunity to learn
Vala, GNOME and the GNU Build Sytem.

$Id$
