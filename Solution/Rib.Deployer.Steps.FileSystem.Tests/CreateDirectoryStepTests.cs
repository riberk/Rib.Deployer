namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using NUnit.Framework;

    [TestFixture]
    public class CreateDirectoryStepTests
    {
        [Test]
        public void ApplyTest()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var creatingDir = new DirectoryInfo(Path.Combine(currentDir, "ApplyTest"));
            DeleteDirIfExists(creatingDir);
            var step = new CreateDirectoryStep(new CreateDirectorySettings(creatingDir.FullName, "dir", false), null);
            Assert.IsFalse(creatingDir.Exists);
            step.Apply();
            creatingDir.Refresh();
            Assert.IsTrue(creatingDir.Exists);
        }

        [Test]
        public void ApplyExistsTest()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var creatingDir = new DirectoryInfo(Path.Combine(currentDir, "ApplyExistsTest"));
            DeleteDirIfExists(creatingDir);
            creatingDir.Create();
            creatingDir.Refresh();
            var step = new CreateDirectoryStep(new CreateDirectorySettings(creatingDir.FullName, "dir", false), null);
            Assert.Throws<InvalidOperationException>(() => step.Apply());
        }

        [Test]
        public void RollbackTest()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var creatingDir = new DirectoryInfo(Path.Combine(currentDir, "RollbackTest"));
            var step = new CreateDirectoryStep(new CreateDirectorySettings(creatingDir.FullName, "dir", false), null);
            DeleteDirIfExists(creatingDir);
            creatingDir.Create();
            creatingDir.Refresh();
            Assert.IsTrue(creatingDir.Exists);
            step.Rollback();
            creatingDir.Refresh();
            Assert.IsFalse(creatingDir.Exists);
        }

        [Test]
        public void RollbackNonRecursiveTest()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var creatingDir = new DirectoryInfo(Path.Combine(currentDir, "RollbackNonRecursiveTest"));
            DeleteDirIfExists(creatingDir);

            var step = new CreateDirectoryStep(new CreateDirectorySettings(creatingDir.FullName, "dir", false), null);
            creatingDir.Create();
            creatingDir.CreateSubdirectory("123");
            creatingDir.Refresh();
            Assert.IsTrue(creatingDir.Exists);
            Assert.Throws<IOException>(() => step.Rollback());
        }

        [Test]
        public void RollbackRecursiveTest()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var creatingDir = new DirectoryInfo(Path.Combine(currentDir, "RollbackRecursiveTest"));
            DeleteDirIfExists(creatingDir);
            var step = new CreateDirectoryStep(new CreateDirectorySettings(creatingDir.FullName, "dir", true), null);
            creatingDir.Create();
            creatingDir.CreateSubdirectory("123");
            creatingDir.Refresh();
            Assert.IsTrue(creatingDir.Exists);
            step.Rollback();
            creatingDir.Refresh();
            Assert.IsFalse(creatingDir.Exists);
        }

        private static void DeleteDirIfExists([NotNull] DirectoryInfo creatingDir)
        {
            if (!creatingDir.Exists) return;
            creatingDir.Delete(true);
            creatingDir.Refresh();
        }

        [Test]
        public void CreateTest()
        {
            var step = CreateDirectoryStep.Create("name", "path", true);
            Assert.IsNotNull(step);
            Assert.IsInstanceOf<CreateDirectoryStep>(step);
        }

        [Test]
        public void CreateWithLoggerTest()
        {
            var step = CreateDirectoryStep.Create("name", "path", null, true);
            Assert.IsNotNull(step);
            Assert.IsInstanceOf<CreateDirectoryStep>(step);
        }
    }
}