namespace Rib.Deployer.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Common.Logging;
    using JetBrains.Annotations;

    public class CspContainerExportSettings : IStepSettings
    {
        public CspContainerExportSettings([NotNull] string name,
                                          bool includePrivate,
                                          [NotNull] string containerName,
                                          CspProviderFlags cspProviderFlags)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(containerName));
            Name = name;
            IncludePrivate = includePrivate;
            ContainerName = containerName;
            CspProviderFlags = cspProviderFlags;
        }

        public string ContainerName { get; }

        public bool IncludePrivate { get; }

        public CspProviderFlags CspProviderFlags { get; }

        public string Name { get; }
    }

    public class CspContainerExportStep : DeployStepBase<CspContainerExportSettings>
    {
        public CspContainerExportStep([NotNull] CspContainerExportSettings settings) : base(settings)
        {
        }

        public CspContainerExportStep([NotNull] CspContainerExportSettings settings, ILog logger) : base(settings, logger)
        {
        }

        public override void Apply()
        {
            var csp = new CspParameters
            {
                Flags = CspProviderFlags.UseExistingKey | Settings.CspProviderFlags
            };
            throw new NotImplementedException();
        }

        public override void Rollback()
        {
            throw new NotImplementedException();
        }
    }


    public class TuppleStep : DeployStepBase<TuppleSettings>, IDisposable
    {
        [NotNull] private readonly List<IDeployStep> _invoked = new List<IDeployStep>();

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public TuppleStep([NotNull] TuppleSettings settings, ILog logger) : base(settings, logger)
        {
        }

        /// <summary>
        ///     Финализировать шаг. Вызывается после применения всех шагов
        /// </summary>
        public void Dispose()
        {
            foreach (var step in _invoked)
            {
                Logger.Info($"Tupple step {step.Name} closing");
                var disposable = step as IDisposable;
                disposable?.Dispose();
                Logger.Info($"Tupple step {step.Name} closed");
            }
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

        public static IDeployStep Create([NotNull] string name, [NotNull] params IDeployStep[] steps)
        {
            return new TuppleStep(new TuppleSettings(name, steps), null);
        }

        public static IDeployStep Create([NotNull] string name, ILog logger, [NotNull] params IDeployStep[] steps)
        {
            return new TuppleStep(new TuppleSettings(name, steps), logger);
        }
    }
}