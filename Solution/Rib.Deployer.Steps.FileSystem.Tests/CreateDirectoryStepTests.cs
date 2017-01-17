namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CreateDirectoryStepTests
    {
        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplyExistsTest()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var creatingDir = new DirectoryInfo(Path.Combine(currentDir, "ApplyExistsTest"));
            DeleteDirIfExists(creatingDir);
            creatingDir.Create();
            creatingDir.Refresh();
            var step = new CreateDirectoryStep(new CreateDirectorySettings(creatingDir.FullName, "dir", false), null);
            step.Apply();
        }

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(IOException))]
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
            step.Rollback();
        }

        [TestMethod]
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

        [TestMethod]
        public void CreateTest()
        {
            var step = CreateDirectoryStep.Create("name", "path", true);
            Assert.IsNotNull(step);
            Assert.IsInstanceOfType(step, typeof(CreateDirectoryStep));
        }

        [TestMethod]
        public void CreateWithLoggerTest()
        {
            var step = CreateDirectoryStep.Create("name", "path", null, true);
            Assert.IsNotNull(step);
            Assert.IsInstanceOfType(step, typeof(CreateDirectoryStep));
        }
    }
}