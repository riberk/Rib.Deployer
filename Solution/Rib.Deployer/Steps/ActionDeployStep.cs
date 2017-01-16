namespace Rib.Deployer.Steps
{
    using System;
    using Common.Logging;
    using JetBrains.Annotations;

    public class ActionDeployStep : DeployStepBase<ActionDeploySettings>, IDisposable
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public ActionDeployStep([NotNull] ActionDeploySettings settings) : base(settings)
        {
        }

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public ActionDeployStep([NotNull] ActionDeploySettings settings, ILog logger) : base(settings, logger)
        {
        }

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            Settings.Apply();
        }

        /// <summary>Откатить шаг</summary>
        public override void Rollback()
        {
            Settings.Rollback();
        }

        /// <summary>
        ///     Финализировать шаг. Вызывается после применения всех шагов
        /// </summary>
        public void Dispose()
        {
            Settings.Close?.Invoke();
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] Action apply, [NotNull] Action rollback, Action close = null)
        {
            return new ActionDeployStep(new ActionDeploySettings(name, apply, rollback, close));
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] Action apply, [NotNull] Action rollback, ILog logger, Action close = null)
        {
            return new ActionDeployStep(new ActionDeploySettings(name, apply, rollback, close), logger);
        }
    }
}