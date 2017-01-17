namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using System.IO;
    using Common.Logging;
    using JetBrains.Annotations;

    public class CreateDirectoryStep : DeployStepBase<CreateDirectorySettings>
    {
        public CreateDirectoryStep([NotNull] CreateDirectorySettings settings, ILog logger) : base(settings, logger)
        {
        }

        public override void Apply()
        {
            Logger.Info($"Create directory {Settings.Path}");
            var di = new DirectoryInfo(Settings.Path);
            if (di.Exists)
            {
                throw new InvalidOperationException("Directory exists");
            }
            di.Create();
        }

        public override void Rollback()
        {
            var di = new DirectoryInfo(Settings.Path);
            if (di.Exists)
            {
                di.Delete(Settings.RecursiveRemoveOnRollback);
            }
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string path, bool recursiveDeleteOnRollback = false)
        {
            return new CreateDirectoryStep(new CreateDirectorySettings(path, name, recursiveDeleteOnRollback), null);
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string path, ILog logger, bool recursiveDeleteOnRollback = false)
        {
            return new CreateDirectoryStep(new CreateDirectorySettings(path, name, recursiveDeleteOnRollback), logger);
        }
    }
}