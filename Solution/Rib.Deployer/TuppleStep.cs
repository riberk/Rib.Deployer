namespace Rib.Deployer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    public class TuppleStep : DeployStepBase<TuppleSettings>
    {
        [NotNull] private readonly List<IDeployStep> _invoked = new List<IDeployStep>();

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public TuppleStep([NotNull] TuppleSettings settings) : base(settings)
        {
        }

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            foreach (var deployStep in Settings.Steps)
            {
                try
                {
                    Logger.Info($"Tupple step {deployStep.Name} applying");
                    deployStep.Apply();
                    Logger.Info($"Tupple step {deployStep.Name} applied");
                    _invoked.Add(deployStep);
                }
                catch (Exception e)
                {
                    Logger.Error($"Tupple step {deployStep.Name} with type {deployStep.GetType()} error", e);
                    Rollback();
                    throw;
                }
            }
        }

        /// <summary>Откатить шаг</summary>
        public override void Rollback()
        {
            foreach (var step in ((IEnumerable<IDeployStep>) _invoked).Reverse())
            {
                Logger.Info($"Tupple step {step.Name} rolling back");
                step.Rollback();
                Logger.Info($"Tupple step {step.Name} rolled back");
            }
        }

        /// <summary>
        ///     Финализировать шаг. Вызывается после применения всех шагов
        /// </summary>
        public override void Close()
        {
            foreach (var step in _invoked)
            {
                Logger.Info($"Tupple step {step.Name} closing");
                step.Close();
                Logger.Info($"Tupple step {step.Name} closed");
            }
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] params IDeployStep[] steps)
        {
            return new TuppleStep(new TuppleSettings(name, steps));
        }
    }
}