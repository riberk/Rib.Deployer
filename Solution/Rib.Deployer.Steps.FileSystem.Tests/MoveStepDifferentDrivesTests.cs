namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using Common.Logging;
    using Moq;
    using NUnit.Framework;
    using TestInfrastructure;

    [TestFixture]
    public class MoveStepDifferentDrivesTests
    {
        private Mock<ILog> _loggerMock;
        private string _srcDir;
        private string _dstDir;
        private const string Src = @"C:\tmp\Rib.Deployer.Tests\MoveStepTests\ApplyWithDifferentDrives\1.txt";
        private const string Dst = @"D:\tmp\Rib.Deployer.Tests\MoveStepTests\ApplyWithDifferentDrives\1.txt";

        [SetUp]
        public void SetUp()
        {
            var deployedPath = TestFsHelper.AbsPath("TestFiles/1.txt");

            _srcDir = Path.GetDirectoryName(Src);
            _dstDir = Path.GetDirectoryName(Dst);
            if (Directory.Exists(_srcDir))
            {
                Directory.Delete(_srcDir, true);
            }
            Directory.CreateDirectory(_srcDir);
            if (Directory.Exists(_dstDir))
            {
                Directory.Delete(_dstDir, true);
            }
            Directory.CreateDirectory(_dstDir);

            File.Copy(deployedPath, Src);
            _loggerMock = new Mock<ILog>(MockBehavior.Loose);
        }

        [Test]
        public void ApplyFileWithDifferentDrives()
        {
            var moveStep = new MoveStep(new HasDestFsSettings("move", Src, Dst), _loggerMock.Object);
            moveStep.Apply();
            Assert.IsFalse(File.Exists(Src));
            Assert.IsTrue(File.Exists(Dst));
            _loggerMock.Verify();
        }

        [Test]
        public void RollbackFileWithDifferentDrives()
        {
            var moveStep = new MoveStep(new HasDestFsSettings("move", Src, Dst), _loggerMock.Object);
            moveStep.Apply();
            Assert.IsFalse(File.Exists(Src));
            Assert.IsTrue(File.Exists(Dst));
            moveStep.Rollback();
            Assert.IsFalse(File.Exists(Dst));
            Assert.IsTrue(File.Exists(Src));
            _loggerMock.Verify();
        }

        [Test]
        public void ApplyDirWithDifferentDrives()
        {
            _loggerMock.Setup(x => x.Warn(It.IsAny<string>())).Verifiable("Logger not warn for different drives");
            var dst = Path.Combine(_dstDir, "ApplyDirWithDifferentDrives");
            var moveStep = new MoveStep(new HasDestFsSettings("move", _srcDir, dst), _loggerMock.Object);
            moveStep.Apply();
            Assert.IsFalse(Directory.Exists(_srcDir));
            Assert.IsTrue(Directory.Exists(dst));
            _loggerMock.Verify();
        }
    }
}