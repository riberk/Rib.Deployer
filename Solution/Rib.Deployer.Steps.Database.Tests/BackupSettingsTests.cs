namespace Rib.Deployer.Steps.Database
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackupSettingsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameNullTest() => new BackupSettings(null, "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameEmptyTest() => new BackupSettings(string.Empty, "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameWhiteSpaceTest() => new BackupSettings("  ", "path");


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsPathNullTest() => new BackupSettings("Name", null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsPathEmptyTest() => new BackupSettings("Name", string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsPathWhiteSpaceTest() => new BackupSettings("Name", "  ");
    }
}