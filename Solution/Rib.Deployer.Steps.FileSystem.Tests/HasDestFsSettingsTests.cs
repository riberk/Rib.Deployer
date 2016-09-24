namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HasDestFsSettingsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasDestFsSettingsNullTest() => new HasDestFsSettings("name", "src", null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasDestFsSettingsEmptyTest() => new HasDestFsSettings("name", "src", string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasDestFsSettingsWhiteSpaceTest() => new HasDestFsSettings("name", "src", "   ");
    }
}