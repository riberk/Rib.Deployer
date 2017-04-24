namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using TestInfrastructure;

    [TestFixture]
    public class FsSettingsTests
    {
        [Test]
        public void FsSettingsTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "Dir");

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

        [Test]
        public void FsSettingsNull1()  => Assert.Throws<ArgumentException>(() =>  new FsSettings(null, "Dir"));

        [Test]
        public void FsSettingsNull2()  => Assert.Throws<ArgumentException>(() =>  new FsSettings(string.Empty, "Dir"));

        [Test]
        public void FsSettingsNull3()  => Assert.Throws<ArgumentException>(() =>  new FsSettings("   ", "Dir"));

        [Test]
        public void FsSettingsNull4()  => Assert.Throws<ArgumentException>(() =>  new FsSettings("name", null));

        [Test]
        public void FsSettingsNull5()  => Assert.Throws<ArgumentException>(() =>  new FsSettings("name", string.Empty));

        [Test]
        public void FsSettingsNull6()  => Assert.Throws<ArgumentException>(() =>  new FsSettings("name", "   "));
    }
}