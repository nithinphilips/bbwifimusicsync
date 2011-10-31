using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using iTuner;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WifiSyncDesktop.Model;

namespace MusicSyncTests
{
    [TestClass]
    public class NotifyPropertyChangesTests
    {
        [TestMethod]
        public void AutoNotifyPropertyChangesAutoPropertiesTest()
        {
            SyncSettings sync = new SyncSettings();
            bool notified = false;
            sync.PropertyChanged += (s, e) => { notified = true; };
            
            sync.Size = 10;
            Assert.IsTrue(notified);

            notified = false;
            sync.Status = "Something";
            Assert.IsTrue(notified);
        }

        // This test will fail when no drives are available.
        [TestMethod]
        public void AutoNotifyPropertyChangesManualPropertiesTest()
        {
            SyncSettings sync = new SyncSettings();
            bool notified = false;

            sync.LoadDrives();
            sync.PropertyChanged += (s, e) => { notified = true; };

            sync.Path = null;
            
            Assert.IsTrue(notified);
        }
    }
}
