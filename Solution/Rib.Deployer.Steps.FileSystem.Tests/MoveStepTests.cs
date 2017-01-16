namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MoveStepTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\1.txt", "MoveFileDir")]
        public void ApplyFileTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "MoveFileDir");
            var newPath = Path.Combine(currentPath);

            var step = new MoveStep(new HasDestFsSettings("move", Path.Combine(path, "1.txt"), Path.Combine(newPath, "1_moved.txt")), null);
            
            step.Apply();

            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "1_moved.txt")));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "MoveDirTestDir")]
        [DeploymentItem("TestFiles/2.txt", "MoveDirTestDir")]
        public void ApplyDirTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "MoveDirTestDir");
            var newPath = Path.Combine(currentPath, "MoveDirTestDir_new");

            Assert.IsTrue(Directory.Exists(path));

            var step = new MoveStep(new HasDestFsSettings("Copy directory", path, newPath), null);
            step.Apply();

            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "2.txt")));
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\1.txt", "RollbackMoveFileDir")]
        public void RollbackFileTest()
        {
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

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "RollbackMoveDirTestDir")]
        [DeploymentItem("TestFiles/2.txt", "RollbackMoveDirTestDir")]
        public void RollbackDirTest()
        {
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


        [TestMethod]
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