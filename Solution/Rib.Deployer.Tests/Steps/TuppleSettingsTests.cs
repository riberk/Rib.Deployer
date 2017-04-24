namespace Rib.Deployer.Steps
{
    using System;
    using NUnit.Framework;
    using Moq;

    [TestFixture]
    public class TuppleSettingsTests
    {
        private Mock<IDeployStep> _step;

        [SetUp]
        public void Init()
        {
            _step = new Mock<IDeployStep>(MockBehavior.Strict);
        }
        
        [Test]
        public void ActionDeploySettingsNull1Test()  => Assert.Throws<ArgumentNullException>(() =>  new TuppleSettings("name", null));

        [Test]
        public void ActionDeploySettingsNull2Test()  => Assert.Throws<ArgumentException>(() =>  new TuppleSettings(null, _step.Object));

        [Test]
        public void ActionDeploySettingsEmptyTest()  => Assert.Throws<ArgumentException>(() =>  new TuppleSettings(string.Empty, _step.Object));

        [Test]
        public void ActionDeploySettingsEmptyArrayTest()  => Assert.Throws<ArgumentException>(() =>  new TuppleSettings("sedg", new IDeployStep[0]));

        [Test]
        public void ActionDeploySettingsWhiteSpaceTest()  => Assert.Throws<ArgumentException>(() =>  new TuppleSettings("      ", _step.Object));
    }
}