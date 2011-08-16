using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libMusicSync.Extensions;
using libMusicSync.iTunesExport.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicSyncTests
{
    [TestClass]
    public class PlaylistNameTests
    {
        [TestMethod]
        public void AlbumPlaylistFileName()
        {
            Playlist p = new Playlist(0, "Test", false, PlaylistKind.Album, new List<ITrack>());
            Assert.AreEqual("Al_Test.hpl", p.GetPlaylistFileName());
        }

        [TestMethod]
        public void ArtistPlaylistFileName()
        {
            Playlist p = new Playlist(0, "Test", false, PlaylistKind.Artist, new List<ITrack>());
            Assert.AreEqual("Ar_Test.hpl", p.GetPlaylistFileName());
        }

        [TestMethod]
        public void NormalPlaylistFileName()
        {
            Playlist p = new Playlist(0, "Test", false, PlaylistKind.Playlist, new List<ITrack>());
            Assert.AreEqual("Test.m3u", p.GetPlaylistFileName());
        }
    }
}
