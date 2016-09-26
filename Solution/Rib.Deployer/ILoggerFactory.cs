namespace Rib.Deployer
{
    using System;
    using Common.Logging;
    using JetBrains.Annotations;

    public interface ILoggerFactory
    {
        [NotNull]
        ILog Create(Type t);
    }
}