namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.ComponentModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IisApplicationSettingsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameNullTest() => new IisApplicationSettings(null, "site", IisApplicationSettings.State.Start, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameEmptyTest()
                => new IisApplicationSettings(string.Empty, "site", IisApplicationSettings.State.Start, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameWhiteSpaceTest()
                => new IisApplicationSettings("   ", "site", IisApplicationSettings.State.Start, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsSiteNullTest() => new IisApplicationSettings("name", null, IisApplicationSettings.State.Start, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsSiteEmptyTest()
                => new IisApplicationSettings("name", string.Empty, IisApplicationSettings.State.Start, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsSiteWhiteSpaceTest()
                => new IisApplicationSettings("name", "   ", IisApplicationSettings.State.Start, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void ActionDeploySettingsEnumIsNotDefinedTest()
                => new IisApplicationSettings("name", "site", (IisApplicationSettings.State) 100, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ActionDeploySettingsWaitOutOfRangeTest()
                => new IisApplicationSettings("name", "site", IisApplicationSettings.State.Start, 0, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ActionDeploySettingsMaxWaitsOutOfRangeTest()
                => new IisApplicationSettings("name", "site", IisApplicationSettings.State.Start, 100, -1);
    }
}