namespace Rib.Deployer
{
    using JetBrains.Annotations;

    internal static class DeployerContext
    {
        [NotNull]
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
    }
}