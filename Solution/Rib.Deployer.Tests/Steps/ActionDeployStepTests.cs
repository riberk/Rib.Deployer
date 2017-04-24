namespace Rib.Deployer.Steps
{
    using NUnit.Framework;

    [TestFixture]
    public class ActionDeployStepTests
    {
        [Test]
        public void ApplyTest()
        {
            var i = 0;
            new ActionDeployStep(new ActionDeploySettings("name", () => i++, () => { })).Apply();
            Assert.AreEqual(1, i);
        }

        [Test]
        public void RollbackTest()
        {
            var i = 0;
            var step = new ActionDeployStep(new ActionDeploySettings("name", () => i++, () => i--));

            step.Apply();
            Assert.AreEqual(1, i);
            step.Rollback();
            Assert.AreEqual(0, i);
        }

        [Test]
        public void CloseTest()
        {
            var i = 0;
            var step = new ActionDeployStep(new ActionDeploySettings("name", () => i++, () => i--, () => i = -1));

            step.Apply();
            Assert.AreEqual(1, i);
            step.Rollback();
            Assert.AreEqual(0, i);
            step.Dispose();
            Assert.AreEqual(-1, i);
        }

        [Test]
        public void CreateTest()
        {
            var step = ActionDeployStep.Create("name", () => { }, () => { });
            Assert.IsNotNull(step as ActionDeployStep);
            Assert.AreEqual("name", step.Name);
        }
    }
}