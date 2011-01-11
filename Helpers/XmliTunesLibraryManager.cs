using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiMusicSync.iTunes;
using System.IO;
using System.Web;
using System.Runtime.Caching;

namespace WifiMusicSync.Helpers
{
    public class XmliTunesLibraryManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(XmliTunesLibraryManager).Name);

        ObjectCache cache = MemoryCache.Default;
        const string CACHE_KEY = "XmliTunesLibrary";
        
        string libraryPath;
        

        public XmliTunesLibraryManager()
            :this(iTunesExport.Parser.LibraryParser.GetDefaultLibraryLocation())
        {
        }

        public XmliTunesLibraryManager(string libraryPath)
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
                    log.Info("Loading iTunes XML library from disk.");
                    long aTick = DateTime.Now.Ticks;
                    library = new XmliTunesLibrary(libraryPath);
                    long bTick = DateTime.Now.Ticks;

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.SlidingExpiration = new TimeSpan(0, 5, 0); // Keep cache for 5 mins.
                    policy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { libraryPath }));
                    policy.RemovedCallback = delegate(CacheEntryRemovedArguments arg)
                    {
                        log.InfoFormat("Evicted {0} from cache (Reason: {1})", arg.CacheItem.Key, arg.RemovedReason);
                        GC.Collect(); // Iffy?
                    };
                    cache.Set(CACHE_KEY, library, policy);

                    log.InfoFormat("Library loaded and cached in {0}ms", (int)new TimeSpan(bTick - aTick).TotalMilliseconds);
                }
                else
                {
                    log.Info("Using cached iTunes XML library");
                }

                return library;
            }
        }

    }
}
