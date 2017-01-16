namespace Rib.Deployer.Steps
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TuppleStepTests
    {
        private MockRepository _mockFactory;

        [TestInitialize]
        public void Init()
        {
            _mockFactory = new MockRepository(MockBehavior.Strict);
        }

        public void TestClean()
        {
            _mockFactory.VerifyAll();
        }

        [TestMethod]
        public void ApplyTest()
        {
            var m1 = _mockFactory.Create<IDeployStep>();
            var m2 = _mockFactory.Create<IDeployStep>();
            m1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            m2.Setup(x => x.Name).Returns("Name 2").Verifiable();
            m1.Setup(x => x.Apply()).Verifiable();
            m2.Setup(x => x.Apply()).Verifiable();

            var step = new TuppleStep(new TuppleSettings("name", m1.Object, m2.Object));

            step.Apply();
        }

        [TestMethod]
        public void ApplyErrorTest()
        {
            var exception = new Exception();
            var m1 = _mockFactory.Create<IDeployStep>();
            var m2 = _mockFactory.Create<IDeployStep>();
            m1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            m2.Setup(x => x.Name).Returns("Name 2").Verifiable();
            m1.Setup(x => x.Apply()).Verifiable();
            m2.Setup(x => x.Apply()).Throws(exception).Verifiable();
            m1.Setup(x => x.Rollback()).Verifiable();


            var step = new TuppleStep(new TuppleSettings("name", m1.Object, m2.Object));
            try
            {
                step.Apply();
            }
            catch (Exception e)
            {
                Assert.AreEqual(exception, e);   
            }
        }

        [TestMethod]
        public void RollbackTest()
        {
            var m1 = _mockFactory.Create<IDeployStep>();
            var m2 = _mockFactory.Create<IDeployStep>();
            m1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            m2.Setup(x => x.Name).Returns("Name 2").Verifiable();
            m1.Setup(x => x.Apply()).Verifiable();
            m2.Setup(x => x.Apply()).Verifiable();
            var step = new TuppleStep(new TuppleSettings("name", m1.Object, m2.Object));

            step.Apply();
            m1.Setup(x => x.Rollback()).Verifiable();
            m2.Setup(x => x.Rollback()).Verifiable();
            step.Rollback();
        }

        [TestMethod]
        public void CloseTest()
        {
            var m1 = _mockFactory.Create<IDeployStep>();
            var m1Disposable = m1.As<IDisposable>();
            var m2 = _mockFactory.Create<IDeployStep>();
            m1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            m2.Setup(x => x.Name).Returns("Name 2").Verifiable();
            m1.Setup(x => x.Apply()).Verifiable();
            m2.Setup(x => x.Apply()).Verifiable();
            var step = new TuppleStep(new TuppleSettings("name", m1.Object, m2.Object));

            step.Apply();
            m1.Setup(x => x.Rollback()).Verifiable();
            m2.Setup(x => x.Rollback()).Verifiable();
            step.Rollback();

            m1Disposable.Setup(x => x.Dispose()).Verifiable();

            step.Dispose();
        }

        [TestMethod]
        public void CreateTest()
        {
            var step = TuppleStep.Create("name", _mockFactory.Create<IDeployStep>().Object);
            Assert.AreEqual("name", step.Name);
            Assert.IsNotNull(step as TuppleStep);
        }
    }
}