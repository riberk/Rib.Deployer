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
        private IisObjectState _beforeApplyState;

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        protected IisObjectStateStep([NotNull] IisApplicationSettings settings, ILog logger) : base(settings, logger)
        {
        }

        internal abstract IIisObject GetObject(string name, ServerManager sm);

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            using (var server = new ServerManager())
            {
                Logger.Debug($"Find {Settings.ObjectName} on local iis");
                var iisObject = GetObject(Settings.ObjectName, server);
                if (iisObject == null) throw new KeyNotFoundException($"Object {Settings.ObjectName} could not be found");
                _beforeApplyState = iisObject.State;
                SetState(iisObject, Settings.NewState);
            }
        }

        /// <summary>Откатить шаг</summary>
        public override void Rollback()
        {
            using (var server = new ServerManager())
            {
                var iisObject = GetObject(Settings.ObjectName, server);
                if (iisObject == null)
                {
                    Logger.Warn($"Site {Settings.ObjectName} is null");
                    return;
                }
                SetState(iisObject, _beforeApplyState);
            }
        }

        private void SetState([NotNull] IIisObject obj, IisObjectState state)
        {
            switch (state)
            {
                case IisObjectState.Started:
                case IisObjectState.Stoped:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            Logger.Debug($"Setting state {state} on {obj.Name}");
            if (obj.State != state)
            {
                obj.SetState(state);
            }
            WaitForState(obj, state, Settings.WaitDuration, Settings.MaxWaits);
        }

        private void WaitForState([NotNull] IIisObject site, IisObjectState state, int waitDuration, int maxWaits)
        {
            var iterations = 0;
            while (site.State != state || iterations > maxWaits)
            {
                Logger.Warn($"{site} not {state}. Wait {waitDuration} on {iterations + 1} iteration");
                Thread.Sleep(waitDuration);
                iterations++;
            }
            if (site.State != state) throw new InvalidOperationException($"Could not set state. {site.State}");
        }

        public static IDeployStep CreateSiteStarter([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Started, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreateSiteStarter([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Started, waitDuration, maxWaits), logger);
        }

        public static IDeployStep CreateSiteStoper([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Stoped, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreateSiteStoper([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisSiteStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Stoped, waitDuration, maxWaits), logger);
        }


        public static IDeployStep CreatePoolStarter([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Started, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreatePoolStarter([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Started, waitDuration, maxWaits), logger);
        }

        public static IDeployStep CreatePoolStoper([NotNull] string name, [NotNull] string siteName, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Stoped, waitDuration, maxWaits), null);
        }

        public static IDeployStep CreatePoolStoper([NotNull] string name, [NotNull] string siteName, ILog logger, int waitDuration = 1000, int maxWaits = 10)
        {
            return new IisPoolStateStep(new IisApplicationSettings(name, siteName, IisObjectState.Stoped, waitDuration, maxWaits), logger);
        }
    }
}