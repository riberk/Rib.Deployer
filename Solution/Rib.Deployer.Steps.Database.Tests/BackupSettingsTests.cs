namespace Rib.Deployer.Steps.Database
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class BackupSettingsTests
    {
        [Test]
        public void ActionDeploySettingsNameNullTest()  => Assert.Throws<ArgumentException>(() =>  new BackupSettings(null, "path"));

        [Test]
        public void ActionDeploySettingsNameEmptyTest()  => Assert.Throws<ArgumentException>(() =>  new BackupSettings(string.Empty, "path"));

        [Test]
        public void ActionDeploySettingsNameWhiteSpaceTest()  => Assert.Throws<ArgumentException>(() =>  new BackupSettings("  ", "path"));


        [Test]
        public void ActionDeploySettingsPathNullTest()  => Assert.Throws<ArgumentException>(() =>  new BackupSettings("Name", null));

        [Test]
        public void ActionDeploySettingsPathEmptyTest()  => Assert.Throws<ArgumentException>(() =>  new BackupSettings("Name", string.Empty));

        [Test]
        public void ActionDeploySettingsPathWhiteSpaceTest()  => Assert.Throws<ArgumentException>(() =>  new BackupSettings("Name", "  "));
    }
}