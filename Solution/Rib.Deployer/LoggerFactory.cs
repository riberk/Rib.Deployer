namespace Rib.Deployer
{
    using System;
    using Common.Logging;
    using Common.Logging.Configuration;
    using Common.Logging.Simple;
    using JetBrains.Annotations;

    internal static class LoggerFactory
    {
        static LoggerFactory()
        {
            var properties = new NameValueCollection {["showDateTime"] = "true"};
            LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter(properties);
        }

        [NotNull]
        public static ILog Create<T>()
        {
            return Create(typeof(T));
        }

        [NotNull]
        public static ILog Create(Type t)
        {
            return LogManager.GetLogger(t);
        }
    }
}