namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.ComponentModel;
    using NUnit.Framework;

    [TestFixture]
    public class IisApplicationSettingsTests
    {
        [Test]
        public void ActionDeploySettingsNameNullTest()  => Assert.Throws<ArgumentException>(() =>  new IisApplicationSettings(null, "site", IisObjectState.Started, 10, 10));

        [Test]
        public void ActionDeploySettingsNameEmptyTest()
                 => Assert.Throws<ArgumentException>(() =>  new IisApplicationSettings(string.Empty, "site", IisObjectState.Started, 10, 10));

        [Test]
        public void ActionDeploySettingsNameWhiteSpaceTest()
                 => Assert.Throws<ArgumentException>(() =>  new IisApplicationSettings("   ", "site", IisObjectState.Started, 10, 10));

        [Test]
        public void ActionDeploySettingsSiteNullTest()  => Assert.Throws<ArgumentException>(() =>  new IisApplicationSettings("name", null, IisObjectState.Started, 10, 10));

        [Test]
        public void ActionDeploySettingsSiteEmptyTest()
                 => Assert.Throws<ArgumentException>(() =>  new IisApplicationSettings("name", string.Empty, IisObjectState.Started, 10, 10));

        [Test]
        public void ActionDeploySettingsSiteWhiteSpaceTest()
                 => Assert.Throws<ArgumentException>(() =>  new IisApplicationSettings("name", "   ", IisObjectState.Started, 10, 10));

        [Test]
        public void ActionDeploySettingsEnumIsNotDefinedTest()
                 => Assert.Throws<InvalidEnumArgumentException>(() =>  new IisApplicationSettings("name", "site", (IisObjectState) 100, 10, 10));

        [Test]
        public void ActionDeploySettingsWaitOutOfRangeTest()
                 => Assert.Throws<ArgumentOutOfRangeException>(() =>  new IisApplicationSettings("name", "site", IisObjectState.Started, 0, 10));

        [Test]
        public void ActionDeploySettingsMaxWaitsOutOfRangeTest()
                 => Assert.Throws<ArgumentOutOfRangeException>(() =>  new IisApplicationSettings("name", "site", IisObjectState.Started, 100, -1));
    }
}