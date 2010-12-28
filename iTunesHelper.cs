using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;

namespace WifiMusicSync
{
    class iTunesHelper
    {

        public static void FuzzyMatchIniTunes(IEnumerable<TrackInfo> tracks, string lookupSourcePlaylistName, int worstConfidence)
        {
            IEnumerable<TrackInfo> _tracks = tracks;

            for (int i = 0; i <= worstConfidence; i++)
            {
                _tracks = MatchIniTunes(_tracks, lookupSourcePlaylistName, i);
            }
        }

        public static List<TrackInfo> MatchIniTunes(IEnumerable<TrackInfo> tracks, string lookupSourcePlaylistName, int confidence)
        {
            List<TrackInfo> unmatchedItems = new List<TrackInfo>();

            Console.WriteLine("Connecting to iTunes...");
            iTunesApp app = new iTunesApp();

            IITSource library = app.Sources.get_ItemByName("Library");
            IITPlaylist lookupSourcePlaylist;


            if (string.IsNullOrEmpty(lookupSourcePlaylistName))
            {
                lookupSourcePlaylistName = "Music";
            }

            lookupSourcePlaylist = library.Playlists.get_ItemByName(lookupSourcePlaylistName);

            if (lookupSourcePlaylist == null)
            {
                lookupSourcePlaylist = library.Playlists.get_ItemByName("Music");
            }


            foreach (IITTrack iTrack in lookupSourcePlaylist.Tracks)
            {
                foreach (var track in tracks)
                {

                    int artistLv = LevenshteinDistance.Compute(track.Artist, iTrack.Artist);
                    int albumLv = LevenshteinDistance.Compute(track.Album, iTrack.Album);
                    int titleLv = LevenshteinDistance.Compute(track.Title, iTrack.Name);
                    bool trackMatches = (track.TrackNumber == iTrack.TrackNumber);

                    if (trackMatches && (artistLv == confidence) || ((albumLv == confidence) && (titleLv == confidence)))
                    {
                        track.MatchConfidence = 100 - (confidence * 10);
                        Console.WriteLine("{0}% Confidence", track.MatchConfidence);
                        track.iTunesTrack = iTrack;
                        track.LocalPath = ((IITFileOrCDTrack)iTrack).Location;
                        track.MatchConfidence = 100 - (confidence * 10);
                    }
                }
            }


            foreach (var track in tracks)
            {
                if (string.IsNullOrEmpty(track.LocalPath))
                {
                    unmatchedItems.Add(track);
                }
            }

            Console.WriteLine("Unmatched: {0}", unmatchedItems.Count);
            return unmatchedItems;
        }
    }
}
