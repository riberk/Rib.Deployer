﻿namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class BackupStepTests
    {
        private const string ConnectionString = "Data Source=CurrentServer;Initial Catalog=master;Integrated Security=True";

        [NotNull] private Mock<IDatabaseInfo> _dbInfoMock;

        [TestInitialize]
        public void TestInit()
        {
            _dbInfoMock = new Mock<IDatabaseInfo>(MockBehavior.Strict);
        }

        [TestMethod]
        public void ApplyTest()
        {
            const string name = "RibDeployerApplyBackupStepTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");

            _dbInfoMock.Setup(x => x.Backup(path)).Verifiable();
            new BackupStep(new BackupSettings("Apply", path), _dbInfoMock.Object).Apply();
            _dbInfoMock.VerifyAll();
        }

        [TestMethod]
        public void RollbackTest()
        {
            const string name = "RibDeployerRollbackWithoutDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");

            var step = new BackupStep(new BackupSettings("Apply", path), _dbInfoMock.Object);

            _dbInfoMock.Setup(x => x.Restore(path)).Verifiable();

            step.Rollback();

            _dbInfoMock.VerifyAll();
        }


        [TestMethod]
        public void CreateTest()
        {
            var step = BackupStep.Create("name", ConnectionString, "path");
            Assert.IsNotNull(step as BackupStep);
            Assert.AreEqual("name", step.Name);
        }

        [TestMethod]
        public void DisposeOwnerTest()
        {
            const string name = "RibDeployerRollbackWithoutDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");
            var step = new BackupStep(new BackupSettings("Apply", path), _dbInfoMock.Object, true);

            _dbInfoMock.Setup(x => x.Dispose()).Verifiable();
            step.Dispose();
            _dbInfoMock.VerifyAll();
        }

        [TestMethod]
        public void DisposeIsNotOwnerTest()
        {
            const string name = "RibDeployerRollbackWithoutDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");
            var step = new BackupStep(new BackupSettings("Apply", path), _dbInfoMock.Object, false);
            step.Dispose();
        }
    }
}