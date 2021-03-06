﻿namespace Rib.Deployer
{
    using System;
    using Common.Logging;
    using JetBrains.Annotations;

    public abstract class DeployStepBase<T> : IDeployStep<T>
        where T : class, IStepSettings
    {
        [NotNull] protected readonly ILog Logger;

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        protected DeployStepBase([NotNull] T settings) : this(settings, null)
        {
        }

        protected DeployStepBase([NotNull] T settings, ILog logger)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            Settings = settings;
            Logger = logger ?? DeployerContext.LoggerFactory.Create(GetType());
        }

        public T Settings { get; }

        public string Name => Settings.Name;

        public abstract void Apply();

        public abstract void Rollback();
    }
}