namespace Rib.Deployer.Steps
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionDeployStepTests
    {
        [TestMethod]
        public void ApplyTest()
        {
            var i = 0;
            new ActionDeployStep(new ActionDeploySettings("name", () => i++, () => { })).Apply();
            Assert.AreEqual(1, i);
        }

        [TestMethod]
        public void RollbackTest()
        {
            var i = 0;
            var step = new ActionDeployStep(new ActionDeploySettings("name", () => i++, () => i--));

            step.Apply();
            Assert.AreEqual(1, i);
            step.Rollback();
            Assert.AreEqual(0, i);
        }

        [TestMethod]
        public void CloseTest()
        {
            var i = 0;
            var step = new ActionDeployStep(new ActionDeploySettings("name", () => i++, () => i--, () => i = -1));

            step.Apply();
            Assert.AreEqual(1, i);
            step.Rollback();
            Assert.AreEqual(0, i);
            step.Close();
            Assert.AreEqual(-1, i);
        }

        [TestMethod]
        public void CreateTest()
        {
            var step = ActionDeployStep.Create("name", () => { }, () => { });
            Assert.IsNotNull(step as ActionDeployStep);
            Assert.AreEqual("name", step.Name);
        }
    }
}