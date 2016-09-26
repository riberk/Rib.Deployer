namespace Rib.Deployer
{
    internal static class DeployerContext
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
    }
}