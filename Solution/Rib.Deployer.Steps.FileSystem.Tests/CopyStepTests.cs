namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CopyStepTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/1.txt")]
        public void ApplyFileTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentPath, "1.txt");
            var newFilePath = Path.Combine(currentPath, "1_copy.txt");

            Assert.IsTrue(File.Exists(filePath));

            new CopyStep(new HasDestFsSettings("Copy file", filePath, newFilePath)).Apply();

            Assert.IsTrue(File.Exists(newFilePath));
            Assert.IsTrue(File.Exists(filePath));
            CollectionAssert.AreEqual(File.ReadAllBytes(filePath), File.ReadAllBytes(newFilePath));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "CopyTestDir")]
        [DeploymentItem("TestFiles/2.txt", "CopyTestDir")]
        public void ApplyDirTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "CopyTestDir");
            var newPath = Path.Combine(currentPath, "CopyTestDir_new");

            Assert.IsTrue(Directory.Exists(path));

            new CopyStep(new HasDestFsSettings("Copy directory", path, newPath)).Apply();

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "2.txt")));
            CollectionAssert.AreEqual(File.ReadAllBytes(Path.Combine(path, "1.txt")), File.ReadAllBytes(Path.Combine(newPath, "1.txt")));
            CollectionAssert.AreEqual(File.ReadAllBytes(Path.Combine(path, "2.txt")), File.ReadAllBytes(Path.Combine(newPath, "2.txt")));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "FileRollback")]
        public void RollbackFileTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentPath, "FileRollback", "1.txt");
            var newFilePath = Path.Combine(currentPath, "FileRollback", "1_copy.txt");

            Assert.IsTrue(File.Exists(filePath));

            var copyStep = new CopyStep(new HasDestFsSettings("Copy file", filePath, newFilePath));
            copyStep.Apply();

            Assert.IsTrue(File.Exists(newFilePath));
            Assert.IsTrue(File.Exists(filePath));
            CollectionAssert.AreEqual(File.ReadAllBytes(filePath), File.ReadAllBytes(newFilePath));

            copyStep.Rollback();

            Assert.IsTrue(File.Exists(filePath));
            Assert.IsFalse(File.Exists(newFilePath));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "CopyTestDirRollback")]
        [DeploymentItem("TestFiles/2.txt", "CopyTestDirRollback")]
        [DeploymentItem("TestFiles/3.txt", "CopyTestDirRollback/subdir")]
        public void RollbackDirTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "CopyTestDirRollback");
            var newPath = Path.Combine(currentPath, "CopyTestDirRollback_new");

            Assert.IsTrue(Directory.Exists(path));

            var copyStep = new CopyStep(new HasDestFsSettings("Copy directory", path, newPath));
            copyStep.Apply();

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "subdir", "3.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(newPath, "subdir", "3.txt")));
            CollectionAssert.AreEqual(File.ReadAllBytes(Path.Combine(path, "1.txt")), File.ReadAllBytes(Path.Combine(newPath, "1.txt")));
            CollectionAssert.AreEqual(File.ReadAllBytes(Path.Combine(path, "2.txt")), File.ReadAllBytes(Path.Combine(newPath, "2.txt")));
            CollectionAssert.AreEqual(File.ReadAllBytes(Path.Combine(path, "subdir", "3.txt")), File.ReadAllBytes(Path.Combine(newPath, "subdir", "3.txt")));

            copyStep.Rollback();

            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(newPath, "1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(newPath, "2.txt")));
            Assert.IsFalse(Directory.Exists(newPath));
        }

        [TestMethod]
        public void CreateTest()
        {
            const string name = "name";
            const string src = "src";
            const string dest = "dest";
            var step = CopyStep.Create(name, src, dest);
            Assert.IsNotNull(step as CopyStep);
            Assert.AreEqual(name, step.Name);
        }
    }
}