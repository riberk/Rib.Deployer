namespace Rib.Deployer.Steps
{
    using System;
    using System.ServiceProcess;
    using Common.Logging;
    using JetBrains.Annotations;

    public class ServiceStateStep : DeployStepBase<ServiceStateSettings>
    {
        public ServiceStateStep([NotNull] ServiceStateSettings settings, ILog logger) : base(settings, logger)
        {
        }

        public override void Apply()
        {
            SetState(Settings.NewState);
        }

        public override void Rollback()
        {
            var rolledBackToState = Settings.NewState == ServiceStateSettings.State.Start
                                        ? ServiceStateSettings.State.Stop
                                        : ServiceStateSettings.State.Start;
            SetState(rolledBackToState);
        }

        public static IDeployStep CreateStarter([NotNull] string name,
                                                [NotNull] string serviceName,
                                                ILog logger,
                                                int timeoutInMilliseconds = 180000)
        {
            var serviceStateSettings = new ServiceStateSettings(name, serviceName, ServiceStateSettings.State.Start, timeoutInMilliseconds);
            return new ServiceStateStep(serviceStateSettings, logger);
        }

        public static IDeployStep CreateStoper([NotNull] string name,
                                               [NotNull] string serviceName,
                                               ILog logger,
                                               int timeoutInMilliseconds = 180000)
        {
            var serviceStateSettings = new ServiceStateSettings(name, serviceName, ServiceStateSettings.State.Stop, timeoutInMilliseconds);
            return new ServiceStateStep(serviceStateSettings, logger);
        }

        private void SetState(ServiceStateSettings.State state)
        {
            using (var sc = new ServiceController(Settings.ServiceName))
            {
                switch (state)
                {
                    case ServiceStateSettings.State.Start:
                        if (sc.Status != ServiceControllerStatus.Running)
                        {
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(Settings.TimeoutInMilliseconds));
                        }
                        break;
                    case ServiceStateSettings.State.Stop:
                        if (sc.Status != ServiceControllerStatus.Stopped)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(Settings.TimeoutInMilliseconds));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}