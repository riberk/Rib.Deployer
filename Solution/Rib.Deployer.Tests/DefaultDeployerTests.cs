namespace Rib.Deployer
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DefaultDeployerTests
    {
        private MockRepository _mockFactory;

        [TestInitialize]
        public void Init()
        {
            _mockFactory = new MockRepository(MockBehavior.Strict);
        }

        [TestCleanup]
        public void Clean()
        {
            _mockFactory.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest() => new DefaultDeployer(null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyTest() => new DefaultDeployer(new IDeployStep[0]);

        [TestMethod]
        public void DeployGoodTest()
        {
            var s1 = _mockFactory.Create<IDeployStep>();
            var s2 = _mockFactory.Create<IDeployStep>();

            s1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            s2.Setup(x => x.Name).Returns("Name 2").Verifiable();

            s1.Setup(x => x.Apply()).Verifiable();
            s2.Setup(x => x.Apply()).Verifiable();

            s1.Setup(x => x.Close()).Verifiable();
            s2.Setup(x => x.Close()).Verifiable();

            new DefaultDeployer(s1.Object, s2.Object).Deploy();
        }

        [TestMethod]
        public void DeployWithRollbackTest()
        {
            var exception = new Exception();
            var s1 = _mockFactory.Create<IDeployStep>();
            var s2 = _mockFactory.Create<IDeployStep>();

            s1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            s2.Setup(x => x.Name).Returns("Name 2").Verifiable();

            s1.Setup(x => x.Apply()).Verifiable();
            s2.Setup(x => x.Apply()).Throws(exception).Verifiable();

            s1.Setup(x => x.Rollback()).Verifiable();

            new DefaultDeployer(s1.Object, s2.Object).Deploy();
        }

        [TestMethod]
        public void DeployWithRollbackExceptionTest()
        {
            var exception = new Exception();
            var rollbackException = new Exception();
            var s1 = _mockFactory.Create<IDeployStep>();
            var s2 = _mockFactory.Create<IDeployStep>();

            s1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            s2.Setup(x => x.Name).Returns("Name 2").Verifiable();

            s1.Setup(x => x.Apply()).Verifiable();
            s2.Setup(x => x.Apply()).Throws(exception).Verifiable();

            s1.Setup(x => x.Rollback()).Throws(rollbackException).Verifiable();

            try
            {
                new DefaultDeployer(s1.Object, s2.Object).Deploy();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(rollbackException, e);
            }
        }

        [TestMethod]
        public void DeployWithCloseExceptionTest()
        {
            var exception = new Exception();
            var s1 = _mockFactory.Create<IDeployStep>();
            var s2 = _mockFactory.Create<IDeployStep>();

            s1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            s2.Setup(x => x.Name).Returns("Name 2").Verifiable();

            s1.Setup(x => x.Apply()).Verifiable();
            s2.Setup(x => x.Apply()).Verifiable();

            s1.Setup(x => x.Close()).Throws(exception).Verifiable();
            s2.Setup(x => x.Close()).Verifiable();
            new DefaultDeployer(s1.Object, s2.Object).Deploy();
        }
    }
}