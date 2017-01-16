namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RestoreStepTests
    {
        [NotNull] private Mock<IDatabaseInfo> _dbMock;

        [TestInitialize]
        public void TestInit()
        {
            _dbMock = new Mock<IDatabaseInfo>();
        }

        [TestMethod]
        public void ApplyExistsTest()
        {
            const string backupPath = "backupPath";
            string backPathForRollback = null;
            _dbMock.Setup(x => x.Name).Returns("DbName");
            _dbMock.Setup(x => x.Exists()).Returns(true).Verifiable();
            _dbMock.Setup(x => x.Backup(It.IsAny<string>())).Callback((string path) => { backPathForRollback = path; }).Verifiable();
            _dbMock.Setup(x => x.Restore(backupPath)).Verifiable();

            var restoreStep = new RestoreStepImpl(new RestoreSettings(backupPath, "name"), _dbMock.Object);
            restoreStep.Apply();

            Assert.IsNotNull(restoreStep.State);
            Assert.IsTrue(restoreStep.State.Exists);
            Assert.AreEqual(backPathForRollback, restoreStep.State.BackupPath);

            _dbMock.VerifyAll();
        }

        [TestMethod]
        public void ApplyNotExistsTest()
        {
            const string backupPath = "backupPath";

            _dbMock.Setup(x => x.Name).Returns("DbName");
            _dbMock.Setup(x => x.Exists()).Returns(false).Verifiable();
            _dbMock.Setup(x => x.Restore(backupPath)).Verifiable();

            var restoreStep = new RestoreStepImpl(new RestoreSettings(backupPath, "name"), _dbMock.Object);
            restoreStep.Apply();

            Assert.IsNotNull(restoreStep.State);
            Assert.IsFalse(restoreStep.State.Exists);

            _dbMock.VerifyAll();
        }

        [TestMethod]
        public void RollbackExistsTest()
        {
            const string backupPath = "backupPath";
            var rollbackBackupPath = Guid.NewGuid().ToString();
            var restoreStep = new RestoreStepImpl(new RestoreSettings(backupPath, "name"), _dbMock.Object,
                                                  new RestoreStep.PreviousState(rollbackBackupPath, true));
            _dbMock.Setup(x => x.Restore(rollbackBackupPath)).Verifiable();

            restoreStep.Rollback();

            _dbMock.VerifyAll();
        }

        [TestMethod]
        public void RollbackNotExistsTest()
        {
            const string backupPath = "backupPath";
            var restoreStep = new RestoreStepImpl(new RestoreSettings(backupPath, "name"), _dbMock.Object,
                                                  new RestoreStep.PreviousState(null, false));
            _dbMock.Setup(x => x.Drop()).Verifiable();

            restoreStep.Rollback();

            _dbMock.VerifyAll();
        }

        [TestMethod]
        public void DisposeNotExistsTest()
        {
            const string backupPath = "backupPath";
            var restoreStep = new RestoreStepImpl(new RestoreSettings(backupPath, "name"), _dbMock.Object,
                                                  new RestoreStep.PreviousState(null, false));
            restoreStep.Dispose();
        }

        [TestMethod]
        public void DisposeExistsTest()
        {
            const string backupPath = "backupPath";
            const string path = "123.txt";
            using (File.Create(path))
            {
            }
            Assert.IsTrue(File.Exists(path));

            var restoreStep = new RestoreStepImpl(new RestoreSettings(backupPath, "name"), _dbMock.Object,
                                                  new RestoreStep.PreviousState(path, true));

            restoreStep.Dispose();

            Assert.IsFalse(File.Exists(path));
        }

        public class RestoreStepImpl : RestoreStep
        {
            public RestoreStepImpl(
                [NotNull] RestoreSettings settings,
                [NotNull] IDatabaseInfo databaseInfo,
                PreviousState state) : base(settings, databaseInfo)
            {
                PrevState = state;
            }

            public RestoreStepImpl(
                [NotNull] RestoreSettings settings,
                [NotNull] IDatabaseInfo databaseInfo) : base(settings, databaseInfo)
            {
            }

            protected sealed override PreviousState PrevState { get; set; }

            public PreviousState State => PrevState;
        }
    }
}