using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WifiMusicSync
{
    public class PlaylistParser
    {
        string deviceRoot = "file:///SDCard/BlackBerry/music/Media Sync/";

        public List<TrackInfo> ParsePlaylist(string data)
        {
            List<TrackInfo> result = new List<TrackInfo>();
            System.IO.StringReader reader = new System.IO.StringReader(data);

            using (reader)
            {
                while (reader.Peek() > 0)
                {
                    String line = reader.ReadLine();
                                        
                    line = Uri.UnescapeDataString(line);
                    Console.WriteLine(line);

                    line = line.Remove(0, deviceRoot.Length);
                    Console.WriteLine(line);

                    string[] parts = line.Split('/');
                    bool isCompliation = parts[0].Equals("Compilations", StringComparison.OrdinalIgnoreCase);

                    Console.WriteLine("IsCompilation: {0}", isCompliation);

                    string track = "0";
                    string title;
                    string artist = "";
                    string album;
                    
                    if (isCompliation)
                    {
                        album = parts[1];

                        string[] track_artist_title = parts[2].Split('-');

                        if (track_artist_title.Length == 2)
                        {
                            track = track_artist_title[0].Trim();
                            // No artist
                            title = track_artist_title[1].Trim();
                        }
                        else
                        {
                             track = track_artist_title[0].Trim();
                             artist = track_artist_title[1].Trim();
                             title = track_artist_title[2].Trim();
                        }

                        

                        Console.WriteLine("Album: {0}", album);
                        Console.WriteLine("Artist: {0}", artist);
                        Console.WriteLine("Track: {0}", track);
                        Console.WriteLine("Title: {0}", title);
                    }
                    else
                    {
                         artist = parts[0];
                         album = parts[1];

                         string[] track_title = parts[2].Split('-');
                         if (track_title.Length == 2)
                         {
                             track = track_title[0].Trim();
                             title = track_title[1].Trim();
                         }
                         else
                         {
                             // No track
                             title = parts[2];
                         }

                        Console.WriteLine("Album: {0}", album);
                        Console.WriteLine("Artist: {0}", artist);
                        Console.WriteLine("Track: {0}", track);
                        Console.WriteLine("Title: {0}", title);
                    }

                    Console.WriteLine("-----------------------------");

                    result.Add(new TrackInfo
                    {
                        Album = album,
                        Artist = artist,
                        Title = Path.GetFileNameWithoutExtension(title),
                        TrackNumber = int.Parse(track)
                    });

                }
            }

            return result;
        }

    }
}
