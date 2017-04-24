namespace Rib.Deployer.Steps.Application
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    internal class SiteObjectAdapter : IisObjectAdapter
    {
        [NotNull] private readonly Site _site;

        public SiteObjectAdapter([NotNull] Site site)
        {
            _site = site ?? throw new ArgumentNullException(nameof(site));
        }

        public override IisObjectState State => MapState(_site.State);

        public override IisObjectState Start() => MapState(_site.Start());

        public override IisObjectState Stop() => MapState(_site.Stop());

        public override string Name => _site.Name;
    }
}