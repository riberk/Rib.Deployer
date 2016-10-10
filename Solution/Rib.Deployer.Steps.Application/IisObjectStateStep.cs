namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    public abstract class IisObjectStateStep : DeployStepBase<IisApplicationSettings>
    {
        internal abstract IIIsObject GetObject(string name, ServerManager sm);

        /// <summary>�������������� ����� ��������� ������ <see cref="T:System.Object" />.</summary>
        protected IisObjectStateStep([NotNull] IisApplicationSettings settings) : base(settings)
        {
        }

        /// <summary>��������� ���</summary>
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>�������� ���</summary>
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
                    default:
                        throw new ArgumentOutOfRangeException();
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
            if (site.State != ObjectState.Stopped)
            {
                throw new InvalidOperationException("Could not stop");
            }
        }

        private void Start([NotNull] IIIsObject site)
        {
            Logger.Debug($"Starting site {site.Name}");
            if (site.State == ObjectState.Stopped)
            {
                site.Start();
            }
            if (site.State != ObjectState.Started)
            {
                throw new InvalidOperationException("Could not start");
            }
        }

        public static IDeployStep CreateSiteStarter([NotNull] string name, [NotNull] string siteName)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Start));
        }

        public static IDeployStep CreateSiteStoper([NotNull] string name, [NotNull] string siteName)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Stop));
        }

        public static IDeployStep CreatePoolStarter([NotNull] string name, [NotNull] string siteName)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Start));
        }

        public static IDeployStep CreatePoolStoper([NotNull] string name, [NotNull] string siteName)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisApplicationSettings.State.Stop));
        }
    }
}