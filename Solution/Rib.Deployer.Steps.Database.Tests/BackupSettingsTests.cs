namespace Rib.Deployer.Steps.Database
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackupSettingsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameNullTest() => new BackupSettings(null, "str", "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameEmptyTest() => new BackupSettings(string.Empty, "str", "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameWhiteSpaceTest() => new BackupSettings("  ", "str", "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsConNullTest() => new BackupSettings("Name", null, "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsConEmptyTest() => new BackupSettings("Name", string.Empty, "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsConWhiteSpaceTest() => new BackupSettings("Name", "   ", "path");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsPathNullTest() => new BackupSettings("Name", "str", null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsPathEmptyTest() => new BackupSettings("Name", "str", string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsPathWhiteSpaceTest() => new BackupSettings("Name", "str", "  ");
    }
}