namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using NUnit.Framework;
    using TestInfrastructure;

    [TestFixture]
    public class MoveStepTests
    {
        [Test]
        public void ApplyFileTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles\\1.txt", "MoveFileDir");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "MoveFileDir");
            var newPath = Path.Combine(currentPath);

            var dest = Path.Combine(newPath, "1_moved.txt");
            var step = new MoveStep(new HasDestFsSettings("move", Path.Combine(path, "1.txt"), dest), null);

            File.Delete(dest);
            step.Apply();

            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(dest));
        }

        [Test]
        public void ApplyDirTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "MoveDirTestDir");
            TestFsHelper.CopyToDirectory("TestFiles/2.txt", "MoveDirTestDir");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "MoveDirTestDir");
            var newPath = Path.Combine(currentPath, "MoveDirTestDir_new");

            Assert.IsTrue(Directory.Exists(path));
            Directory.Delete(newPath, true);
            var step = new MoveStep(new HasDestFsSettings("Copy directory", path, newPath), null);
            step.Apply();

            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "2.txt")));
        }

        [Test]
        public void RollbackFileTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "RollbackMoveFileDir");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RollbackMoveFileDir");
            var newPath = Path.Combine(currentPath);

            var step = new MoveStep(new HasDestFsSettings("move", Path.Combine(path, "1.txt"), Path.Combine(newPath, "1_rollback_moved.txt")), null);

            step.Apply();

            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "1_moved.txt")));

            step.Rollback();

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(newPath, "1_rollback_moved.txt")));
        }

        [Test]
        public void RollbackDirTest()
        {
            TestFsHelper.CopyToDirectory("TestFiles/1.txt", "RollbackMoveDirTestDir");
            TestFsHelper.CopyToDirectory("TestFiles/2.txt", "RollbackMoveDirTestDir");

            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RollbackMoveDirTestDir");
            var newPath = Path.Combine(currentPath, "RollbackMoveDirTestDir_new");

            Assert.IsTrue(Directory.Exists(path));

            var step = new MoveStep(new HasDestFsSettings("Copy directory", path, newPath), null);
            step.Apply();

            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "2.txt")));

            step.Rollback();

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(newPath, "1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(newPath, "2.txt")));
        }


        [Test]
        public void CreateTest()
        {
            const string name = "name";
            const string src = "src";
            const string dest = "dest";
            var step = MoveStep.Create(name, src, dest);
            Assert.IsNotNull(step as MoveStep);
            Assert.AreEqual(name, step.Name);
        }
    }
}