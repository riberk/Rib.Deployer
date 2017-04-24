namespace Rib.Deployer.Steps.Application
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PoolObjectTest
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestnullArg() => new PoolObjectAdapter(null);
    }
}