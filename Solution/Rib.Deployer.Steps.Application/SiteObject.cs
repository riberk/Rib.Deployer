namespace Rib.Deployer.Steps.Application
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    internal class SiteObject : IIIsObject
    {
        [NotNull] private readonly Site _site;

        public SiteObject([NotNull] Site site)
        {
            if (site == null) throw new ArgumentNullException(nameof(site));
            _site = site;
        }

        public ObjectState State => _site.State;

        public ObjectState Start() => _site.Start();

        public ObjectState Stop() => _site.Stop();

        public string Name => _site.Name;
    }
}