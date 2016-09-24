using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rib.Deployer.Steps.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;

    [TestClass]
    public class RemoveStepTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "RemovedItems")]
        public void ApplyTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RemovedItems");

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            var step = new RemoveStep(new FsSettings("remove", Path.Combine(path, "1.txt")));
            step.Apply();
            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "DirRemovedItems")]
        [DeploymentItem("TestFiles/2.txt", "DirRemovedItems")]
        public void ApplyDirTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "DirRemovedItems");

            Assert.IsTrue(Directory.Exists(path));
            var step = new RemoveStep(new FsSettings("remove", path));
            step.Apply();
            Assert.IsFalse(Directory.Exists(path));
        }


        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "RollbackRemovedItems")]
        public void RollbackTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RollbackRemovedItems");

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            var step = new RemoveStep(new FsSettings("remove", Path.Combine(path, "1.txt")));
            step.Apply();
            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            step.Rollback();
            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "RollbackDirRemovedItems")]
        [DeploymentItem("TestFiles/2.txt", "RollbackDirRemovedItems")]
        public void RollbackDirTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RollbackDirRemovedItems");

            Assert.IsTrue(Directory.Exists(path));
            var step = new RemoveStep(new FsSettings("remove", path));
            step.Apply();
            Assert.IsFalse(Directory.Exists(path));
            step.Rollback();
            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "CloseRemovedItems")]
        public void CloseTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "CloseRemovedItems");

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            var step = new RemoveStep(new FsSettings("remove", Path.Combine(path, "1.txt")));
            step.Apply();
            Assert.IsFalse(File.Exists(Path.Combine(path, "1.txt")));
            step.Close();
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "ClosekDirRemovedItems")]
        [DeploymentItem("TestFiles/2.txt", "ClosekDirRemovedItems")]
        public void CloseDirTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "ClosekDirRemovedItems");

            Assert.IsTrue(Directory.Exists(path));
            var step = new RemoveStep(new FsSettings("remove", path));
            step.Apply();
            Assert.IsFalse(Directory.Exists(path));
            step.Close();
        }

        [TestMethod]
        public void CreateTest()
        {
            const string name = "name";
            const string src = "src";
            const string dest = "dest";
            var step = RemoveStep.Create(name, src);
            Assert.IsNotNull(step as RemoveStep);
            Assert.AreEqual(name, step.Name);
        }
    }
}