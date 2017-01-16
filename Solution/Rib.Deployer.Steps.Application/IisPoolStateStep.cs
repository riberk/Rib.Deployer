namespace Rib.Deployer.Steps.Application
{
    using Common.Logging;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    internal class IisPoolStateStep : IisObjectStateStep
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.Object" />.</summary>
        public IisPoolStateStep([NotNull] IisApplicationSettings settings, ILog logger) : base(settings, logger)
        {
        }

        internal override IIIsObject GetObject(string name, ServerManager sm)
        {
            var site = sm?.ApplicationPools?[name];
            return site != null ? new PoolObject(site) : null;
        }
    }
}