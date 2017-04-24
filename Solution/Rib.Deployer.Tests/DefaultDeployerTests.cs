namespace Rib.Deployer
{
    using System;
    using Common.Logging;
    using NUnit.Framework;
    using Moq;

    [TestFixture]
    public class DefaultDeployerTests
    {
        private MockRepository _mockFactory;

        [SetUp]
        public void Init()
        {
            _mockFactory = new MockRepository(MockBehavior.Strict);
        }

        [TearDown]
        public void Clean()
        {
            _mockFactory.VerifyAll();
        }

        [Test]
        public void NullTest() => Assert.Throws<ArgumentNullException>(() => new DefaultDeployer((ILog)null, null));

        [Test]
        public void EmptyTest() => Assert.Throws<ArgumentException>(() => new DefaultDeployer(new IDeployStep[0]));

        [Test]
        public void DeployGoodTest()
        {
            var s1 = _mockFactory.Create<IDeployStep>();
            var s2 = _mockFactory.Create<IDeployStep>();

            s1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            s2.Setup(x => x.Name).Returns("Name 2").Verifiable();

            s1.Setup(x => x.Apply()).Verifiable();
            s2.Setup(x => x.Apply()).Verifiable();

            var s1Disp = s1.As<IDisposable>();
            s1Disp.Setup(x => x.Dispose()).Verifiable();

            new DefaultDeployer(s1.Object, s2.Object).Deploy();
        }

        [Test]
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

            try
            {
                new DefaultDeployer(s1.Object, s2.Object).Deploy();
            }
            catch (Exception e)
            {
                Assert.AreEqual(exception, e);
            }
        }

        [Test]
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

        [Test]
        public void DeployWithCloseExceptionTest()
        {
            var exception = new Exception();
            var s1 = _mockFactory.Create<IDeployStep>();
            var s2 = _mockFactory.Create<IDeployStep>();

            s1.Setup(x => x.Name).Returns("Name 1").Verifiable();
            s2.Setup(x => x.Name).Returns("Name 2").Verifiable();

            s1.Setup(x => x.Apply()).Verifiable();
            s2.Setup(x => x.Apply()).Verifiable();

            var s1Disp = s1.As<IDisposable>();
            var s2Disp = s2.As<IDisposable>();

            s1Disp.Setup(x => x.Dispose()).Throws(exception).Verifiable();
            s2Disp.Setup(x => x.Dispose()).Verifiable();
            new DefaultDeployer(s1.Object, s2.Object).Deploy();
        }
    }
}