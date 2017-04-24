namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using NUnit.Framework;
    using TestInfrastructure;

    [TestFixture]
    public class RemoveStepTests
    {
        [Test]
        public void ApplyTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "RemovedItems");
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RemovedItems");

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            var step = new RemoveStep(new FsSettings("remove", Path.Combine(path, "1.txt")), null);
            step.Apply();
            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
        }

        [Test]
        public void ApplyDirTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "DirRemovedItems");
            TestFsHelper.CopyToDirectory("TestFiles/2.txt", "DirRemovedItems");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "DirRemovedItems");

            Assert.IsTrue(Directory.Exists(path));
            var step = new RemoveStep(new FsSettings("remove", path), null);
            step.Apply();
            Assert.IsFalse(Directory.Exists(path));
        }


        [Test]
        public void RollbackTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "RollbackRemovedItems");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RollbackRemovedItems");

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            var step = new RemoveStep(new FsSettings("remove", Path.Combine(path, "1.txt")), null);
            step.Apply();
            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            step.Rollback();
            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
        }

        [Test]
        public void RollbackDirTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "RollbackDirRemovedItems");
            TestFsHelper.CopyToDirectory("TestFiles/2.txt", "RollbackDirRemovedItems");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RollbackDirRemovedItems");

            Assert.IsTrue(Directory.Exists(path));
            var step = new RemoveStep(new FsSettings("remove", path), null);
            step.Apply();
            Assert.IsFalse(Directory.Exists(path));
            step.Rollback();
            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
        }

        [Test]
        public void CloseTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "CloseRemovedItems");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "CloseRemovedItems");

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            var step = new RemoveStep(new FsSettings("remove", Path.Combine(path, "1.txt")), null);
            step.Apply();
            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            step.Dispose();
        }

        [Test]
        public void CloseDirTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "ClosekDirRemovedItems");
            TestFsHelper.CopyToDirectory("TestFiles/2.txt", "ClosekDirRemovedItems");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "ClosekDirRemovedItems");

            Assert.IsTrue(Directory.Exists(path));
            var step = new RemoveStep(new FsSettings("remove", path), null);
            step.Apply();
            Assert.IsFalse(Directory.Exists(path));
            step.Dispose();
        }

        [Test]
        public void CreateTest()
        {
            const string name = "name";
            const string src = "src";
            var step = RemoveStep.Create(name, src);
            Assert.IsNotNull(step as RemoveStep);
            Assert.AreEqual(name, step.Name);
        }
    }
}