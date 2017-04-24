namespace Rib.Deployer
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class DeployStepBaseTests
    {
        [Test]
        public void DeployStepBaseTest()  => Assert.Throws<ArgumentNullException>(() =>  new DeployStep(null));


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