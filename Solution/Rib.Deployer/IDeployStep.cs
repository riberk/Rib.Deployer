﻿namespace Rib.Deployer
{
    using JetBrains.Annotations;

    public interface IDeployStep
    {
        [NotNull]
        string Name { get; }

        /// <summary>
        /// Применить шаг
        /// </summary>
        void Apply();

        /// <summary>
        /// Откатить шаг
        /// </summary>
        void Rollback();
    }

    public interface IDeployStep<out T> : IDeployStep
            where T : class, IStepSettings
    {
        [NotNull]
        T Settings { get; }
    }
}