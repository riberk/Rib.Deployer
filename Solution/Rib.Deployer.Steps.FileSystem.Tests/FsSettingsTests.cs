namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FsSettingsTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "Dir")]
        public void FsSettingsTest()
        {
            var dirSettings = new FsSettings("name", "Dir");

            Assert.IsTrue(dirSettings.SrcIsDirectory);
            Assert.IsNotNull(dirSettings.SrcInfo as DirectoryInfo);
            Assert.AreEqual("name", dirSettings.Name);
            Assert.AreEqual("Dir", dirSettings.Src);


            var fileSettings = new FsSettings("name", "Dir\\1.txt");

            Assert.IsFalse(fileSettings.SrcIsDirectory);
            Assert.IsNotNull(fileSettings.SrcInfo as FileInfo);
            Assert.AreEqual("name", fileSettings.Name);
            Assert.AreEqual("Dir\\1.txt", fileSettings.Src);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FsSettingsNull1() => new FsSettings(null, "Dir");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FsSettingsNull2() => new FsSettings(string.Empty, "Dir");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FsSettingsNull3() => new FsSettings("   ", "Dir");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FsSettingsNull4() => new FsSettings("name", null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FsSettingsNull5() => new FsSettings("name", string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FsSettingsNull6() => new FsSettings("name", "   ");
    }
}