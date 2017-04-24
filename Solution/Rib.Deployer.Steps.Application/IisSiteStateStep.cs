namespace Rib.Deployer.Steps.Application
{
    using Common.Logging;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    internal class IisSiteStateStep : IisObjectStateStep
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public IisSiteStateStep([NotNull] IisApplicationSettings settings, ILog logger) : base(settings, logger)
        {
        }

        internal override IIisObject GetObject(string name, ServerManager sm)
        {
            var site = sm?.Sites?[name];
            return site != null ? new SiteObjectAdapter(site) : null;
        }
    }
}