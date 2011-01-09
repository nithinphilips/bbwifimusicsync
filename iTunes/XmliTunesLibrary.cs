using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesExport.Parser;
using WifiMusicSync.Helpers;

namespace WifiMusicSync.iTunes
{
    public class XmliTunesLibrary : iTunesLibrary
    {
        LibraryParser parser;

        public XmliTunesLibrary()
            : this(iTunesExport.Parser.LibraryParser.GetDefaultLibraryLocation())
        { }
        
        public XmliTunesLibrary(string xmlLibraryPath)
        {
            parser = new LibraryParser(xmlLibraryPath);
            CanModify = false;
            MusicFolderPath = parser.MusicFolder;
            Playlists = parser.Playlists;
        }

    }
}
