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
        public void ActionDeploySettingsNameNullTest() => new IisApplicationSettings(null, "site", IisObjectState.Started, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameEmptyTest()
                => new IisApplicationSettings(string.Empty, "site", IisObjectState.Started, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNameWhiteSpaceTest()
                => new IisApplicationSettings("   ", "site", IisObjectState.Started, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsSiteNullTest() => new IisApplicationSettings("name", null, IisObjectState.Started, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsSiteEmptyTest()
                => new IisApplicationSettings("name", string.Empty, IisObjectState.Started, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsSiteWhiteSpaceTest()
                => new IisApplicationSettings("name", "   ", IisObjectState.Started, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void ActionDeploySettingsEnumIsNotDefinedTest()
                => new IisApplicationSettings("name", "site", (IisObjectState) 100, 10, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ActionDeploySettingsWaitOutOfRangeTest()
                => new IisApplicationSettings("name", "site", IisObjectState.Started, 0, 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ActionDeploySettingsMaxWaitsOutOfRangeTest()
                => new IisApplicationSettings("name", "site", IisObjectState.Started, 100, -1);
    }
}