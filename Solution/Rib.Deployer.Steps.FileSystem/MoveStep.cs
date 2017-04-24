namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using Common.Logging;
    using JetBrains.Annotations;

    public class MoveStep : DeployStepBase<HasDestFsSettings>
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.Object" />.</summary>
        public MoveStep([NotNull] HasDestFsSettings settings, ILog logger) : base(settings, logger)
        {
        }

        public override void Apply()
        {
            Logger.Debug($"Move {Settings.Src} to {Settings.Dest}");
            if (Settings.SrcIsDirectory)
            {
                Logger.Debug("Src is directory");
                MoveDirectory(Settings.Src, Settings.Dest);
            }
            else
            {
                Logger.Debug("Src is file");
                (Settings.SrcInfo as FileInfo).MoveTo(Settings.Dest);
            }
        }

        private void MoveDirectory([NotNull] string from, [NotNull] string to)
        {
            if (Path.GetPathRoot(from) == Path.GetPathRoot(to))
            {
                Directory.Move(from, to);
                return;
            }

            Logger.Warn("Src and dest on different drives. Using copy and delete");
            var fromDir = new DirectoryInfo(@from);
            FsHelper.CopyDirectory(fromDir, new DirectoryInfo(to));
            fromDir.Delete(true);
        }

        public override void Rollback()
        {
            Logger.Info($"Rollback move {Settings.Dest} to {Settings.Src}");
            if (Settings.SrcIsDirectory)
            {
                Logger.Debug("Src is directory");
                Directory.Move(Settings.Dest, Settings.Src);
            }
            else
            {
                Logger.Debug("Src is file");
                File.Move(Settings.Dest, Settings.Src);
            }
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string src, [NotNull] string dest)
        {
            return new MoveStep(new HasDestFsSettings(name, src, dest), null);
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string src, [NotNull] string dest, ILog logger)
        {
            return new MoveStep(new HasDestFsSettings(name, src, dest), logger);
        }
    }
}