namespace Rib.Deployer.Steps.Application
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SiteObjectTest
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestnullArg() => new SiteObject(null);
    }
}