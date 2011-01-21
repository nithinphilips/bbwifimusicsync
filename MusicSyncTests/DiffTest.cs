using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using libMusicSync.Helpers;
using libMusicSync.Model;
using NUnit.Framework;

namespace MusicSyncTests
{
    [TestFixture]
    public class DiffTest
    {
        [Test]
        public void TestDiffAdd()
        {
            List<string> a = new List<string> { "Line1", "Line2", "Line3", "Line4", "Line5" };
            List<string> b = new List<string> { "Line1", "Line2", "Line3" };

            var result = DiffHandler.Diff(a, b);

            foreach (SyncAction syncAction in result)
            {
                Assert.IsTrue(syncAction.Type == SyncType.Add);
                Assert.IsTrue(syncAction.DeviceLocation == "Line4" || syncAction.DeviceLocation == "Line5");
            }
        }

        [Test]
        public void TestDiffRemove()
        {
            List<string> a = new List<string> { "Line1", "Line2", "Line3", "Line4", "Line5" };
            List<string> b = new List<string> { "Line1", "Line2", "Line3" };

            var result = DiffHandler.Diff(b, a);

            foreach (SyncAction syncAction in result)
            {
                Assert.IsTrue(syncAction.Type == SyncType.Remove);
                Assert.IsTrue(syncAction.DeviceLocation == "Line4" || syncAction.DeviceLocation == "Line5");
            }
        }

        [Test]
        public void TestDiffEmptyArgument1()
        {
            List<string> a = new List<string> { "Line1", "Line2", "Line3", "Line4", "Line5" };
            List<string> b = new List<string> { };

            var result = DiffHandler.Diff(a, b);

            foreach (SyncAction syncAction in result)
            {
                Assert.IsTrue(syncAction.Type == SyncType.Add);
                Assert.IsTrue(
                    syncAction.DeviceLocation == "Line1" || 
                    syncAction.DeviceLocation == "Line2" || 
                    syncAction.DeviceLocation == "Line3" || 
                    syncAction.DeviceLocation == "Line4" || 
                    syncAction.DeviceLocation == "Line5");
            }
        }

        [Test]
        public void TestDiffEmptyArgument2()
        {
            List<string> a = new List<string> { "Line1", "Line2", "Line3", "Line4", "Line5" };
            List<string> b = new List<string> { };

            var result = DiffHandler.Diff(b, a);

            foreach (SyncAction syncAction in result)
            {
                Assert.IsTrue(syncAction.Type == SyncType.Remove);
                Assert.IsTrue(
                    syncAction.DeviceLocation == "Line1" ||
                    syncAction.DeviceLocation == "Line2" ||
                    syncAction.DeviceLocation == "Line3" ||
                    syncAction.DeviceLocation == "Line4" ||
                    syncAction.DeviceLocation == "Line5");
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDiffNullArgument1()
        {
            DiffHandler.Diff(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDiffNullArgument2()
        {
            DiffHandler.Diff(new List<string>(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDiffNullArgument3()
        {
            DiffHandler.Diff(null, new List<string>());
        }

    }
}
