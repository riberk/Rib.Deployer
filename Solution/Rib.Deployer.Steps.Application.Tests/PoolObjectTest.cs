namespace Rib.Deployer.Steps.Application
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class PoolObjectTest
    {
        [Test] 
        public void TestnullArg() => Assert.Throws<ArgumentNullException>(() => new PoolObjectAdapter(null));
    }
}