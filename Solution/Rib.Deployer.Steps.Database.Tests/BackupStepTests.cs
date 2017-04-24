namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using NUnit.Framework;
    using Moq;

    [TestFixture]
    public class BackupStepTests
    {
        private const string ConnectionString = "Data Source=CurrentServer;Initial Catalog=master;Integrated Security=True";

        [NotNull] private Mock<IDatabaseInfo> _dbInfoMock;

        [SetUp]
        public void TestInit()
        {
            _dbInfoMock = new Mock<IDatabaseInfo>(MockBehavior.Strict);
        }

        [Test]
        public void ApplyTest()
        {
            const string name = "RibDeployerApplyBackupStepTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");

            _dbInfoMock.Setup(x => x.Backup(path, 10)).Verifiable();
            new BackupStep(new BackupSettings("Apply", path) {CommandTimeout = 10}, _dbInfoMock.Object).Apply();
            _dbInfoMock.VerifyAll();
        }

        [Test]
        public void RollbackTest()
        {
            const string name = "RibDeployerRollbackWithoutDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");

            var step = new BackupStep(new BackupSettings("Apply", path), _dbInfoMock.Object);

            _dbInfoMock.Setup(x => x.Restore(path, 30)).Verifiable();

            step.Rollback();

            _dbInfoMock.VerifyAll();
        }


        [Test]
        public void CreateTest()
        {
            var step = BackupStep.Create("name", ConnectionString, "path", 10);
            var backupStep = step as BackupStep;
            Assert.IsNotNull(backupStep);
            Assert.AreEqual("name", step.Name);
            Assert.AreEqual(10, backupStep.Settings.CommandTimeout);
        }

        [Test]
        public void DisposeOwnerTest()
        {
            const string name = "RibDeployerRollbackWithoutDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");
            var step = new BackupStep(new BackupSettings("Apply", path), _dbInfoMock.Object, true);

            _dbInfoMock.Setup(x => x.Dispose()).Verifiable();
            step.Dispose();
            _dbInfoMock.VerifyAll();
        }

        [Test]
        public void DisposeIsNotOwnerTest()
        {
            const string name = "RibDeployerRollbackWithoutDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");
            var step = new BackupStep(new BackupSettings("Apply", path), _dbInfoMock.Object, false);
            step.Dispose();
        }
    }
}