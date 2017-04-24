namespace Rib.Deployer.Steps
{
    using System;
    using JetBrains.Annotations;

    public class ServiceStateSettings : IStepSettings
    {
        public ServiceStateSettings(
            [NotNull] string name,
            [NotNull] string serviceName,
            WindowsServiceState newState,
            int timeoutInMilliseconds)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceName));
            if (timeoutInMilliseconds <= 0) throw new ArgumentOutOfRangeException(nameof(timeoutInMilliseconds));
            Name = name;
            ServiceName = serviceName;
            NewState = newState;
            TimeoutInMilliseconds = timeoutInMilliseconds;
        }

        [NotNull]
        public string ServiceName { get; }

        public WindowsServiceState NewState { get; }

        public int TimeoutInMilliseconds { get; }

        public string Name { get; }
    }
}