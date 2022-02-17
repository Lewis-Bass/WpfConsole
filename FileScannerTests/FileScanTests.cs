using Common.Settings;
using FileScaner.Scan;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FileScannerTests
{
    [TestClass]
    public class FileScanTests
    {
        [TestMethod]
        public void LoadEverything()
        {
            AutoLoadSettings settings = AutoLoadSettings.Load(true);
            settings.AutoLoadLastProcessed = new System.DateTime(1, 1, 1);
            var filescan = new FileScan(settings);
            filescan.ExecuteScan();

            var newSettings = AutoLoadSettings.Load(true);
            Assert.IsTrue(newSettings.AutoLoadLastProcessed.ToShortDateString() == DateTime.Now.ToShortDateString());
            Assert.IsTrue(newSettings.AutoLoadLastTotalUpload > 0);
        }

        [TestMethod]
        public void LoadChanges()
        {
            AutoLoadSettings settings = AutoLoadSettings.Load(true);
            settings.AutoLoadLastProcessed = DateTime.Now.AddDays(-10);
            var filescan = new FileScan(settings);
            filescan.ExecuteScan();

            var newSettings = AutoLoadSettings.Load(true);
            Assert.IsTrue(newSettings.AutoLoadLastProcessed.ToShortDateString() == DateTime.Now.ToShortDateString());
            Assert.IsTrue(newSettings.AutoLoadLastTotalUpload > 0);
        }

        [TestMethod]
        public void LoadNothing()
        {
            AutoLoadSettings settings = AutoLoadSettings.Load(true);
            settings.AutoLoadLastProcessed = DateTime.Now.AddDays(10);
            var filescan = new FileScan(settings);
            filescan.ExecuteScan();

            var newSettings = AutoLoadSettings.Load(true);
            Assert.IsTrue(newSettings.AutoLoadLastProcessed.ToShortDateString() == DateTime.Now.ToShortDateString());
            Assert.IsTrue(newSettings.AutoLoadLastTotalUpload == 0);
        }
    }
}
