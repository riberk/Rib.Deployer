namespace Rib.Deployer
{
    using System;
    using Common.Logging;
    using Common.Logging.Configuration;
    using Common.Logging.Simple;

    internal class LoggerFactory : ILoggerFactory
    {
        public LoggerFactory()
        {
            if (LogManager.Adapter == null || LogManager.Adapter is NoOpLoggerFactoryAdapter)
            {
                var properties = new NameValueCollection {["showDateTime"] = "true"};
                LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter(properties);
            }
        }

        public ILog Create(Type t)
        {
            return LogManager.GetLogger(t);
        }
    }
}