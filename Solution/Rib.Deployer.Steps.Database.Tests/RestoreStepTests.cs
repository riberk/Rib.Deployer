namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using NUnit.Framework;
    using Moq;

    [TestFixture]
    public class RestoreStepTests
    {
        private const string ConnectionString = "Data Source=CurrentServer;Initial Catalog=master;Integrated Security=True";

        [NotNull] private Mock<IDatabaseInfo> _dbMock;

        [SetUp]
        public void TestInit()
        {
            _dbMock = new Mock<IDatabaseInfo>();
        }

        [Test]
        public void ApplyExistsTest()
        {
            const string backupPath = "backupPath";
            string backPathForRollback = null;
            _dbMock.Setup(x => x.Name).Returns("DbName");
            _dbMock.Setup(x => x.Exists()).Returns(true).Verifiable();
            _dbMock.Setup(x => x.Backup(It.IsAny<string>(), RestoreSettings.DefaultCommandTimeout)).Callback((string path, int timeout) => { backPathForRollback = path; }).Verifiable();
            _dbMock.Setup(x => x.Restore(backupPath, RestoreSettings.DefaultCommandTimeout)).Verifiable();

            var restoreStep = new RestoreStepImpl(new RestoreSettings("name", backupPath), _dbMock.Object);
            restoreStep.Apply();

            Assert.IsNotNull(restoreStep.State);
            Assert.IsTrue(restoreStep.State.Exists);
            Assert.AreEqual(backPathForRollback, restoreStep.State.BackupPath);

            _dbMock.VerifyAll();
        }

        [Test]
        public void ApplyNotExistsTest()
        {
            const string backupPath = "backupPath";

            _dbMock.Setup(x => x.Name).Returns("DbName");
            _dbMock.Setup(x => x.Exists()).Returns(false).Verifiable();
            _dbMock.Setup(x => x.Restore(backupPath, RestoreSettings.DefaultCommandTimeout)).Verifiable();

            var restoreStep = new RestoreStepImpl(new RestoreSettings("name", backupPath), _dbMock.Object);
            restoreStep.Apply();

            Assert.IsNotNull(restoreStep.State);
            Assert.IsFalse(restoreStep.State.Exists);

            _dbMock.VerifyAll();
        }

        [Test]
        public void RollbackExistsTest()
        {
            const string backupPath = "backupPath";
            var rollbackBackupPath = Guid.NewGuid().ToString();
            var restoreStep = new RestoreStepImpl(new RestoreSettings("name", backupPath), _dbMock.Object,
                                                  new RestoreStep.PreviousState(rollbackBackupPath, true));
            _dbMock.Setup(x => x.Restore(rollbackBackupPath, RestoreSettings.DefaultCommandTimeout)).Verifiable();

            restoreStep.Rollback();

            _dbMock.VerifyAll();
        }

        [Test]
        public void RollbackNotExistsTest()
        {
            const string backupPath = "backupPath";
            var restoreStep = new RestoreStepImpl(new RestoreSettings("name", backupPath), _dbMock.Object,
                                                  new RestoreStep.PreviousState(null, false));
            _dbMock.Setup(x => x.Drop()).Verifiable();

            restoreStep.Rollback();

            _dbMock.VerifyAll();
        }

        [Test]
        public void DisposeNotExistsTest()
        {
            const string backupPath = "backupPath";
            var restoreStep = new RestoreStepImpl(new RestoreSettings("name", backupPath), _dbMock.Object,
                                                  new RestoreStep.PreviousState(null, false));
            restoreStep.Dispose();
        }

        [Test]
        public void DisposeExistsTest()
        {
            const string backupPath = "backupPath";
            const string path = "123.txt";
            using (File.Create(path))
            {
            }
            Assert.IsTrue(File.Exists(path));

            var restoreStep = new RestoreStepImpl(new RestoreSettings("name", backupPath), _dbMock.Object,
                                                  new RestoreStep.PreviousState(path, true));

            restoreStep.Dispose();

            Assert.IsFalse(File.Exists(path));
        }

        [Test]
        public void CreateTest()
        {
            var step = RestoreStep.Create("name", ConnectionString, "path", 50);
            var restoreStep = step as RestoreStep;
            Assert.IsNotNull(restoreStep);
            Assert.AreEqual("name", step.Name);
            Assert.AreEqual(50, restoreStep.Settings.CommandTimeout);
        }

        [Test]
        public void DisposeOwnerTest()
        {
            var step = new RestoreStepImpl(new RestoreSettings("Apply", "123"), _dbMock.Object, new RestoreStep.PreviousState(null, false),
                                           true);

            _dbMock.Setup(x => x.Dispose()).Verifiable();
            step.Dispose();
            _dbMock.VerifyAll();
        }

        [Test]
        public void DisposeIsNotOwnerTest()
        {
            var step = new RestoreStepImpl(new RestoreSettings("Apply", "123"), _dbMock.Object, new RestoreStep.PreviousState(null, false),
                                           false);
            step.Dispose();
        }

        public class RestoreStepImpl : RestoreStep
        {
            public RestoreStepImpl(
                [NotNull] RestoreSettings settings,
                [NotNull] IDatabaseInfo databaseInfo,
                PreviousState state,
                bool isDbOwner = false) : base(settings, databaseInfo, isDbOwner)
            {
                PrevState = state;
            }

            public RestoreStepImpl(
                [NotNull] RestoreSettings settings,
                [NotNull] IDatabaseInfo databaseInfo,
                bool isDbOwner = false) : base(settings, databaseInfo, isDbOwner)
            {
            }

            protected sealed override PreviousState PrevState { get; set; }

            public PreviousState State => PrevState;
        }
    }
}