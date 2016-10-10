namespace Rib.Deployer.Steps.Application
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Web.Administration;

    internal class PoolObject : IIIsObject
    {
        [NotNull]
        private readonly ApplicationPool _pool;

        public PoolObject([NotNull] ApplicationPool pool)
        {
            if (pool == null) throw new ArgumentNullException(nameof(pool));
            _pool = pool;
        }

        public ObjectState State => _pool.State;

        public ObjectState Start() => _pool.Start();

        public ObjectState Stop() => _pool.Stop();

        public string Name => _pool.Name;
    }
}