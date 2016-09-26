namespace Rib.Deployer.Steps
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TuppleSettingsTests
    {
        private Mock<IDeployStep> _step;

        [TestInitialize]
        public void Init()
        {
            _step = new Mock<IDeployStep>(MockBehavior.Strict);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActionDeploySettingsNull1Test() => new TuppleSettings("name", null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNull2Test() => new TuppleSettings(null, _step.Object);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsEmptyTest() => new TuppleSettings(string.Empty, _step.Object);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsEmptyArrayTest() => new TuppleSettings("sedg", new IDeployStep[0]);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsWhiteSpaceTest() => new TuppleSettings("      ", _step.Object);
    }
}