namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    public class IisStateStep : DeployStepBase<IisApplicationSettings>
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public IisStateStep([NotNull] IisApplicationSettings settings) : base(settings)
        {
        }

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            using (var server = new ServerManager())
            {
                Logger.Debug($"Find {Settings.Site} on local iis");
                var site = server.Sites?[Settings.Site];
                if (site == null)
                {
                    throw new KeyNotFoundException($"Site {Settings.Site} could not be found");
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
                var site = server.Sites?[Settings.Site];
                if (site == null)
                {
                    Logger.Warn($"Site {Settings.Site} is null");
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

        private void Stop([NotNull] Site site)
        {
            Logger.Debug($"Stoping site {site.Name}");
            if (site.State == ObjectState.Started)
            {
                site.Stop();
            }
            if (site.State != ObjectState.Stopped)
            {
                throw new InvalidOperationException("Could not stop website");
            }
        }

        private void Start([NotNull] Site site)
        {
            Logger.Debug($"Starting site {site.Name}");
            if (site.State == ObjectState.Stopped)
            {
                site.Start();
            }
            if (site.State != ObjectState.Started)
            {
                throw new InvalidOperationException("Could not start website");
            }
        }

        public static IDeployStep CreateStarter([NotNull] string name, [NotNull] string siteName)
        {
            return new IisStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Start));
        }

        public static IDeployStep CreateStoper([NotNull] string name, [NotNull] string siteName)
        {
            return new IisStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Stop));
        }
    }
}