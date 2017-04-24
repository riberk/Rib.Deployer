namespace Rib.Deployer.Steps.Application
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class SiteObjectTest
    {
        [Test]
        public void TestnullArg() => Assert.Throws<ArgumentNullException>(() => new SiteObjectAdapter(null));
    }
}