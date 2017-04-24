namespace Rib.Deployer.Steps.Application
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IisObjectStateStepTests
    {
        [TestMethod]
        public void CreateSiteStarterTest()
        {
            var res = IisObjectStateStep.CreateSiteStarter("start", "site");
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Started, iisSiteStateStep.Settings.NewState);
        }

        [TestMethod]
        public void CreateSiteStarterWithLogTest()
        {
            var res = IisObjectStateStep.CreateSiteStarter("start", "site", null);
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Started, iisSiteStateStep.Settings.NewState);
        }

        [TestMethod]
        public void CreateSiteStoperTest()
        {
            var res = IisObjectStateStep.CreateSiteStoper("start", "site");
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisSiteStateStep.Settings.NewState);
        }

        [TestMethod]
        public void CreateSiteStoperWithLogTest()
        {
            var res = IisObjectStateStep.CreateSiteStoper("start", "site", null);
            var iisSiteStateStep = res as IisSiteStateStep;
            Assert.IsNotNull(iisSiteStateStep);
            Assert.AreEqual("start", iisSiteStateStep.Name);
            Assert.AreEqual("site", iisSiteStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisSiteStateStep.Settings.NewState);
        }

        [TestMethod]
        public void CreatePoolStoperTest()
        {
            var res = IisObjectStateStep.CreatePoolStoper("start", "site");
            var iisPoolStateStep = res as IisPoolStateStep;
            Assert.IsNotNull(iisPoolStateStep);
            Assert.AreEqual("start", iisPoolStateStep.Name);
            Assert.AreEqual("site", iisPoolStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisPoolStateStep.Settings.NewState);
        }

        [TestMethod]
        public void CreatePoolStoperWithLoggerTest()
        {
            var res = IisObjectStateStep.CreatePoolStoper("start", "site", null);
            var iisPoolStateStep = res as IisPoolStateStep;
            Assert.IsNotNull(iisPoolStateStep);
            Assert.AreEqual("start", iisPoolStateStep.Name);
            Assert.AreEqual("site", iisPoolStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Stoped, iisPoolStateStep.Settings.NewState);
        }

        [TestMethod]
        public void CreatePoolStarterTest()
        {
            var res = IisObjectStateStep.CreatePoolStarter("start", "site");
            var iisPoolStateStep = res as IisPoolStateStep;
            Assert.IsNotNull(iisPoolStateStep);
            Assert.AreEqual("start", iisPoolStateStep.Name);
            Assert.AreEqual("site", iisPoolStateStep.Settings.ObjectName);
            Assert.AreEqual(IisObjectState.Started, iisPoolStateStep.Settings.NewState);
        }

        [TestMethod]
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