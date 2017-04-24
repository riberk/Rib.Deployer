namespace Rib.Deployer.Steps.Application
{
    using NUnit.Framework;

    [TestFixture]
    public class IisObjectStateStepTests
    {
        [Test]
        public void CreateSiteStarterTest()
        {
            var res = IisObjectStateStep.CreateSiteStarter("start", "site");
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Started, iisSiteStateStep.Settings.NewState);
        }

        [Test]
        public void CreateSiteStarterWithLogTest()
        {
            var res = IisObjectStateStep.CreateSiteStarter("start", "site", null);
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Started, iisSiteStateStep.Settings.NewState);
        }

        [Test]
        public void CreateSiteStoperTest()
        {
            var res = IisObjectStateStep.CreateSiteStoper("start", "site");
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisSiteStateStep.Settings.NewState);
        }

        [Test]
        public void CreateSiteStoperWithLogTest()
        {
            var res = IisObjectStateStep.CreateSiteStoper("start", "site", null);
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisSiteStateStep.Settings.NewState);
        }

        [Test]
        public void CreatePoolStoperTest()
        {
            var res = IisObjectStateStep.CreatePoolStoper("start", "site");
            var iisPoolStateStep = res as IisPoolStateStep;
            Assert.IsNotNull(iisPoolStateStep);
            Assert.AreEqual("start", iisPoolStateStep.Name);
            Assert.AreEqual("site", iisPoolStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisPoolStateStep.Settings.NewState);
        }

        [Test]
        public void CreatePoolStoperWithLoggerTest()
        {
            var res = IisObjectStateStep.CreatePoolStoper("start", "site", null);
            var iisPoolStateStep = res as IisPoolStateStep;
            Assert.IsNotNull(iisPoolStateStep);
            Assert.AreEqual("start", iisPoolStateStep.Name);
            Assert.AreEqual("site", iisPoolStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisPoolStateStep.Settings.NewState);
        }

        [Test]
        public void CreatePoolStarterTest()
        {
            var res = IisObjectStateStep.CreatePoolStarter("start", "site");
            var iisPoolStateStep = res as IisPoolStateStep;
            Assert.IsNotNull(iisPoolStateStep);
            Assert.AreEqual("start", iisPoolStateStep.Name);
            Assert.AreEqual("site", iisPoolStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Started, iisPoolStateStep.Settings.NewState);
        }

        [Test]
        public void CreatePoolStarterWithLoggerTest()
        {
            var res = IisObjectStateStep.CreatePoolStarter("start", "site", null);
            var iisPoolStateStep = res as IisPoolStateStep;
            Assert.IsNotNull(iisPoolStateStep);
            Assert.AreEqual("start", iisPoolStateStep.Name);
            Assert.AreEqual("site", iisPoolStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Started, iisPoolStateStep.Settings.NewState);
        }
    }
}