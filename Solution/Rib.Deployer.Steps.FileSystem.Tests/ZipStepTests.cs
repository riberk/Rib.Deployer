namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ZipStepTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "ZipStep")]
        [DeploymentItem("TestFiles/2.txt", "ZipStep")]
        [DeploymentItem("TestFiles/3.txt", "ZipStep")]
        public void ApplyTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "ZipStep");
            var newPath = Path.Combine(currentPath, "apply_zip.zip");

            var step = new ZipStep(new HasDestFsSettings("zip", path, newPath), null);
            step.Apply();

            Assert.IsTrue(File.Exists(newPath));
            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "3.txt")));

            var unzipped = Path.Combine(currentPath, "unzipped");
            Directory.CreateDirectory(unzipped);

            ZipFile.ExtractToDirectory(newPath, unzipped, Encoding.UTF8);

            var unzippedFiles = Directory.GetFiles(unzipped);
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var zippedFile = unzippedFiles.Single(x => Path.GetFileName(file) == Path.GetFileName(x));
                CollectionAssert.AreEqual(File.ReadAllBytes(file), File.ReadAllBytes(zippedFile));
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles/1.txt", "RollbackZipStep")]
        [DeploymentItem("TestFiles/2.txt", "RollbackZipStep")]
        [DeploymentItem("TestFiles/3.txt", "RollbackZipStep")]
        public void RollbackTest()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "RollbackZipStep");
            var newPath = Path.Combine(currentPath, "rollback_zip.zip");

            var step = new ZipStep(new HasDestFsSettings("zip", path, newPath), null);
            step.Apply();

            Assert.IsTrue(File.Exists(newPath));
            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "3.txt")));

            step.Rollback();

            Assert.IsFalse(File.Exists(newPath));
            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "2.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "3.txt")));
        }

        [TestMethod]
        public void CreateTest()
        {
            const string name = "name";
            const string src = "src";
            const string dest = "dest";
            var step = ZipStep.Create(name, src, dest);
            Assert.IsNotNull(step as ZipStep);
            Assert.AreEqual(name, step.Name);
        }
    }
}