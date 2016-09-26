namespace Rib.Deployer
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DeployStepBaseTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeployStepBaseTest() => new DeployStep(null);

        [TestMethod]
        public void CloseTest()
        {
            new DeployStep(new Settings()).Close();
        }

        private class Settings : IStepSettings
        {
            public string Name { get; } = "name";
        }

        private class DeployStep : DeployStepBase<Settings>
        {
            /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
            public DeployStep(Settings settings) : base(settings)
            {
            }

            /// <summary>Применить шаг</summary>
            public override void Apply()
            {
                throw new NotImplementedException();
            }

            /// <summary>Откатить шаг</summary>
            public override void Rollback()
            {
                throw new NotImplementedException();
            }
        }
    }
}