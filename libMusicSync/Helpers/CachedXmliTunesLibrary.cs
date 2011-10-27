//#define HEAVY_LOGGING // Uncomment for even more logging.

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
using System.Linq;
using System.Linq.Expressions;
using libMusicSync.iTunes;
using System.Runtime.Caching;
using log4net;

namespace libMusicSync.Helpers
{
    public class CachedXmliTunesLibrary
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CachedXmliTunesLibrary).Name);

        ObjectCache cache = MemoryCache.Default;
        const string CACHE_KEY = "XmliTunesLibrary";
        
        string libraryPath;
        

        public CachedXmliTunesLibrary()
            :this(iTunesExport.Parser.LibraryParser.GetDefaultLibraryLocation())
        {
        }

        public CachedXmliTunesLibrary(string libraryPath)
        {
            this.libraryPath = libraryPath;
        }


        public XmliTunesLibrary Library
        {
            get
            {
                XmliTunesLibrary library = cache[CACHE_KEY] as XmliTunesLibrary;

                if (library == null)
                {
                    
                    // Only load libray if it has been modified.
                    Log.Info("Loading iTunes XML library from disk.");
                    long aTick = DateTime.Now.Ticks;
                    library = new XmliTunesLibrary(libraryPath);
                    long bTick = DateTime.Now.Ticks;

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.SlidingExpiration = new TimeSpan(0, 10, 0); // Keep cache for 10 mins.
                    policy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { libraryPath }));
                    policy.RemovedCallback = delegate(CacheEntryRemovedArguments arg)
                                                 {
                                                     Log.InfoFormat("Evicted {0} from cache (Reason: {1})", arg.CacheItem.Key, arg.RemovedReason);
                                                     //GC.Collect(); // Iffy?
                                                 };
                    cache.Set(CACHE_KEY, library, policy);


                    Log.InfoFormat("Library loaded and cached in {0}ms", (int)new TimeSpan(bTick - aTick).TotalMilliseconds);

#if HEAVY_LOGGING
                    using (ThreadContext.Stacks["NDC"].Push("Albums"))
                    {
                        Log.Debug(library.Albums.Count() + " Albums Loaded");
                        foreach (var playlist in library.Albums)
                        {
                            Log.Debug(string.Format("{0,4}", playlist.Tracks.Count()) + " in " + playlist.Name);
                        }
                    }
                    
                    using (ThreadContext.Stacks["NDC"].Push("Artists"))
                    {
                        Log.Debug(library.Artists.Count() + " Artists Loaded");
                        foreach (var playlist in library.Artists)
                        {
                            Log.Debug(string.Format("{0,4}", playlist.Tracks.Count()) + " in " + playlist.Name);
                        }
                    }

                    using (ThreadContext.Stacks["NDC"].Push("Playlists"))
                    {
                        Log.Debug(library.Playlists.Count() + " Playlists Loaded");
                        foreach (var playlist in library.Playlists)
                        {
                            Log.Debug(string.Format("{0,4}", playlist.Tracks.Count()) + " in " + playlist.Name);
                        }
                    }
#endif
                }
                else
                {
                    Log.Info("Using cached iTunes XML library");
                }

                return library;
            }
        }

    }
}
