using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesExport.Parser;
using iTunesLib;
using WifiMusicSync.Helpers;
using System.IO;

namespace WifiMusicSync.iTunes
{
    public class ComiTunesLibrary : IiTunesLibrary
    {
        iTunesApp app;

        public ComiTunesLibrary()
        {
            app = new iTunesApp();
            CanModify = true;
            MusicFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            
        }

        public bool CanModify { get; private set; }
        public string MusicFolderPath { get; private set; }
        public IEnumerable<IPlaylist> Playlists { get; private set; }

        public IEnumerable<IPlaylist> GetPlaylists()
        {
            List<Playlist> playlists = new List<Playlist>();
            IITSource library = app.Sources.get_ItemByName("Library");

            foreach (IITPlaylist item in library.Playlists)
            {
                List<Track> tracks = new List<Track>();

                foreach (IITTrack track in item.Tracks)
	            {
                    string location = "";
                    if(track is IITFileOrCDTrack)
                    {
                        IITFileOrCDTrack ctrack = track as IITFileOrCDTrack;
                        location = ctrack.Location;
                    }

		            //tracks.Add(new Track(track.trackID.ToString(), track.Name, track.Artist, track.Duration, location, true, !track.Enabled));
	            }

                Playlist playlist = new Playlist(item.playlistID, item.Name, false, tracks.ToArray());
            }


            return playlists;
        }

        public static List<string> ToPlaylist(IPlaylist playlist, string deviceRoot)
        {
            List<string> result = new List<string>();

            foreach (IITTrack iTrack in playlist.Tracks)
            {
                string playlistStr;

                //string albumArtist = string.IsNullOrEmpty(((IITFileOrCDTrack)iTrack).AlbumArtist) ? ((IITFileOrCDTrack)iTrack).Artist : ((IITFileOrCDTrack)iTrack).AlbumArtist;
                string albumArtist = ((IITFileOrCDTrack)iTrack).AlbumArtist;

                bool isCompilation = (string.IsNullOrEmpty(albumArtist) && iTrack.Compilation);

                string artist = string.IsNullOrEmpty(((IITFileOrCDTrack)iTrack).AlbumArtist) ? ((IITFileOrCDTrack)iTrack).Artist : ((IITFileOrCDTrack)iTrack).AlbumArtist;

                if (isCompilation) { artist = "Compilations"; }

                playlistStr = string.Format("{0}/{1}/{2}",
                         Utilities.MakeFileNameSafe(artist),
                         Utilities.MakeFileNameSafe(iTrack.Album),
                         Path.GetFileName(((IITFileOrCDTrack)iTrack).Location));

                string playlistLine = deviceRoot + playlistStr;
                result.Add(playlistLine);
                //lookupTable.Add(playlistLine, (IITFileOrCDTrack)iTrack);
            }

            return result;
        }


        public List<string> GeneratePlaylist(IPlaylist playlist, string root)
        {
            throw new NotImplementedException();
        }

        public ITrack GetTrack(string playlistLine)
        {
            throw new NotImplementedException();
        }
    }
}
