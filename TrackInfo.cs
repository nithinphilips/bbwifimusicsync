using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;

namespace WifiMusicSync
{
    public class TrackInfo
    {
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public int TrackNumber { get; set; }

        public int MatchConfidence { get; set; }

        public string LocalPath { get; set; }
        public IITTrack iTunesTrack { get; set; }

        public override string ToString()
        {
            return string.Format("[{5}] {0} by {1} from {2} (#{3})\n > {4}", Title, Artist, Album, TrackNumber, LocalPath, MatchConfidence);
        }
    }
}
