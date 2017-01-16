namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Common.Logging;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    public abstract class IisObjectStateStep : DeployStepBase<IisApplicationSettings>
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        protected IisObjectStateStep([NotNull] IisApplicationSettings settings, ILog logger) : base(settings, logger)
        {
        }

        internal abstract IIIsObject GetObject(string name, ServerManager sm);

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            using (var server = new ServerManager())
            {
                Logger.Debug($"Find {Settings.ObjectName} on local iis");
                var site = GetObject(Settings.ObjectName, server);
                if (site == null)
                {
                    throw new KeyNotFoundException($"Site {Settings.ObjectName} could not be found");
                }
                switch (Settings.NewState)
                {
                    case IisApplicationSettings.State.Start:
                        Start(site);
                        break;
                    case IisApplicationSettings.State.Stop:
                        Stop(site);
                        break;
                }
            }
        }

        /// <summary>Откатить шаг</summary>
        public override void Rollback()
        {
            using (var server = new ServerManager())
            {
                var site = GetObject(Settings.ObjectName, server);
                if (site == null)
                {
                    Logger.Warn($"Site {Settings.ObjectName} is null");
                    return;
                }
                switch (Settings.NewState)
                {
                    case IisApplicationSettings.State.Start:
                        Stop(site);
                        break;
                    case IisApplicationSettings.State.Stop:
                        Start(site);
                        break;
                }
            }
        }

        private void Stop([NotNull] IIIsObject site)
        {
            Logger.Debug($"Stoping {site.Name}");
            if (site.State == ObjectState.Started)
            {
                site.Stop();
            }
            EnsureState(site, ObjectState.Stopped, Settings.WaitDuration, Settings.MaxWaits);
        }

        private void Start([NotNull] IIIsObject site)
        {
            Logger.Debug($"Starting site {site.Name}");
            if (site.State == ObjectState.Stopped)
            {
                site.Start();
            }
            EnsureState(site, ObjectState.Started, Settings.WaitDuration, Settings.MaxWaits);
        }

        private void EnsureState([NotNull] IIIsObject site, ObjectState state, int waitDuration = 1000, int maxWaits = 5)
        {
            var iterations = 0;
            while (site.State != state || iterations > maxWaits)
            {
                Logger.Warn($"{site} not {state}. Wait {waitDuration} on {iterations + 1} iteration");
                Thread.Sleep(waitDuration);
                iterations++;
            }
            if (site.State != state)
            {
                throw new InvalidOperationException($"Could not start. {site.State}");
            }
        }

        public static IDeployStep CreateSiteStarter([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Start, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreateSiteStarter([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Start, waitDuration, maxWaits), logger);
        }

        public static IDeployStep CreateSiteStoper([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Stop, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreateSiteStoper([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Stop, waitDuration, maxWaits), logger);
        }


        public static IDeployStep CreatePoolStarter([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Start, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreatePoolStarter([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Start, waitDuration, maxWaits), logger);
        }

        public static IDeployStep CreatePoolStoper([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Stop, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreatePoolStoper([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Stop, waitDuration, maxWaits), logger);
        }
    }
}