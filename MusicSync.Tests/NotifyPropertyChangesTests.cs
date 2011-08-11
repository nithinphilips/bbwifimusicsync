using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WifiSyncDesktop.Model;

namespace MusicSyncTests
{
    [TestClass]
    public class NotifyPropertyChangesTests
    {
        [TestMethod]
        public void AutoNotifyPropertyChangesTest()
        {
            SyncSettings sync = new SyncSettings();
            bool notified = false;
            sync.PropertyChanged += (s, e) => { notified = true; };
            sync.Size = 10;
            Assert.IsTrue(notified);
        }

        [TestMethod]
        public void AutoNotifyPropertyChangesTest2()
        {
            NOtifiable sync = new NOtifiable();
            bool notified = false;
            ((INotifyPropertyChanged)sync).PropertyChanged += (s, e) => { notified = true; };
            sync.Something = "10";
            Assert.IsTrue(notified);
        }
    }
}
