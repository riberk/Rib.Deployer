namespace Rib.Deployer.Steps
{
    using System;
    using System.ServiceProcess;
    using Common.Logging;
    using JetBrains.Annotations;

    public class ServiceStateStep : DeployStepBase<ServiceStateSettings>
    {
        private WindowsServiceState _beforeApplyState;

        public ServiceStateStep([NotNull] ServiceStateSettings settings, ILog logger) : base(settings, logger)
        {
        }

        [Obsolete("Only for tests")]
        internal void SetBeforeApplyState(WindowsServiceState newState)
        {
            _beforeApplyState = newState;
        }

        public override void Apply()
        {
            _beforeApplyState = GetState();
            if (Settings.NewState == _beforeApplyState)
            {
                Logger.Info($"Service allready in state {Settings.NewState}. Apply skipping");
                return;
            }
            SetState(Settings.NewState);
        }

        public override void Rollback()
        {
            var currentState = GetState();
            if (currentState == _beforeApplyState)
            {
                Logger.Info($"State before apply is {_beforeApplyState}. Service allready in state {_beforeApplyState}. Rollback skipping");
                return;
            }
            SetState(_beforeApplyState);
        }

        public static IDeployStep CreateStarter([NotNull] string name,
                                                [NotNull] string serviceName,
                                                ILog logger,
                                                int timeoutInMilliseconds = 180000)
        {
            var serviceStateSettings = new ServiceStateSettings(name, serviceName, WindowsServiceState.Started, timeoutInMilliseconds);
            return new ServiceStateStep(serviceStateSettings, logger);
        }

        public static IDeployStep CreateStoper([NotNull] string name,
                                               [NotNull] string serviceName,
                                               ILog logger,
                                               int timeoutInMilliseconds = 180000)
        {
            var serviceStateSettings = new ServiceStateSettings(name, serviceName, WindowsServiceState.Stoped, timeoutInMilliseconds);
            return new ServiceStateStep(serviceStateSettings, logger);
        }

        private void SetState(WindowsServiceState state)
        {
            using (var sc = new ServiceController(Settings.ServiceName))
            {
                switch (state)
                {
                    case WindowsServiceState.Started:
                        if (sc.Status != ServiceControllerStatus.Running)
                        {
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(Settings.TimeoutInMilliseconds));
                        }
                        break;
                    case WindowsServiceState.Stoped:
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

        private WindowsServiceState GetState()
        {
            using (var sc = new ServiceController(Settings.ServiceName))
            {
                switch (sc.Status)
                {
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.StopPending:
                        throw NotSupportedStateException(sc.Status);
                    case ServiceControllerStatus.Running:
                        return WindowsServiceState.Started;
                    case ServiceControllerStatus.Stopped:
                        return WindowsServiceState.Stoped;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [NotNull]
        private NotSupportedException NotSupportedStateException(ServiceControllerStatus stat)
        {
            return new NotSupportedException(
                $"Status {stat} not supported; Only {ServiceControllerStatus.Running} & {ServiceControllerStatus.Stopped} status supported");
        }
    }
}