Wifi Music Sync
===============

A Client-Server program to keep the music on your Blackberry 
(and potentially other devices with Wifi) updated.

Why?
----
Well, Blackberry Desktop already implements a sweet music sync
option. Why reinvent the proverbial wheel? you may ask. Well, 
the answer is because of the way the BB Desktop sync works,
if you have a very large library (like I do), the program 
will not do much beyond making your Blackberry grid to a halt.

Wifi Music Sync only pushes out only the songs and playlists
you want, while trying to do as much work as it can on the 
server side. Also, even when you only want to sync manually,
the entire Blackberry Desktop Software takes f-o-r-e-v-e-r to 
load, so with the Music Sync Desktop program, which will load 
rather quickly you can easily keep your music updated even 
without Wifi (Music Sync Desktop is intended for first-time
syncing of a large number of songs.)

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
    -> Get the free Community Edition
 * Kayak Server Fx (source included)
 * Log4net (>= 1.2.9.0) (binary included)
 * iTunes (>= 9.0) <http://www.apple.com/itunes/>

$Id$
