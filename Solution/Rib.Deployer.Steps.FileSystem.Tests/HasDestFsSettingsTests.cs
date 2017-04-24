namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class HasDestFsSettingsTests
    {
        [Test]
        public void HasDestFsSettingsNullTest()  => Assert.Throws<ArgumentException>(() =>  new HasDestFsSettings("name", "src", null));

        [Test]
        public void HasDestFsSettingsEmptyTest()  => Assert.Throws<ArgumentException>(() =>  new HasDestFsSettings("name", "src", string.Empty));

        [Test]
        public void HasDestFsSettingsWhiteSpaceTest()  => Assert.Throws<ArgumentException>(() =>  new HasDestFsSettings("name", "src", "   "));
    }
}