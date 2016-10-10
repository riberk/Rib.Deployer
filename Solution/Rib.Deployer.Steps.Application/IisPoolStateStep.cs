namespace Rib.Deployer.Steps.Application
{
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    internal class IisPoolStateStep : IisObjectStateStep
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.Object" />.</summary>
        public IisPoolStateStep([NotNull] IisApplicationSettings settings) : base(settings)
        {
        }

        internal override IIIsObject GetObject(string name, ServerManager sm)
        {
            var site = sm?.ApplicationPools?[name];
            return site != null ? new PoolObject(site) : null;
        }
    }
}