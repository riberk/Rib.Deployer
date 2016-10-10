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
            Assert.IsNotNull(res as IisSiteStateStep);
            Assert.AreEqual("start", res.Name);
        }

        [TestMethod]
        public void CreateSiteStoperTest()
        {
            var res = IisObjectStateStep.CreateSiteStoper("start", "site");
            Assert.IsNotNull(res as IisSiteStateStep);
            Assert.AreEqual("start", res.Name);
        }

        [TestMethod]
        public void CreatePoolStarterTest()
        {
            var res = IisObjectStateStep.CreatePoolStarter("start", "site");
            Assert.IsNotNull(res as IisPoolStateStep);
            Assert.AreEqual("start", res.Name);
        }

        [TestMethod]
        public void CreatePoolStoperTest()
        {
            var res = IisObjectStateStep.CreatePoolStoper("start", "site");
            Assert.IsNotNull(res as IisPoolStateStep);
            Assert.AreEqual("start", res.Name);
        }


    }
}