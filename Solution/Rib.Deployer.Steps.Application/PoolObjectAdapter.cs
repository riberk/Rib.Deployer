namespace Rib.Deployer.Steps.Application
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    internal class PoolObjectAdapter : IisObjectAdapter
    {
        [NotNull]
        private readonly ApplicationPool _pool;

        public PoolObjectAdapter([NotNull] ApplicationPool pool)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
        }

        public override IisObjectState State => MapState(_pool.State);

        public override IisObjectState Start() => MapState(_pool.Start());

        public override IisObjectState Stop() => MapState(_pool.Stop());



        public override string Name => _pool.Name;
    }
}