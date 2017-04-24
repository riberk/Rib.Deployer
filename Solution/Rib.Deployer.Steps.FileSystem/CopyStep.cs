namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using Common.Logging;
    using JetBrains.Annotations;

    public class CopyStep : DeployStepBase<HasDestFsSettings>
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public CopyStep([NotNull] HasDestFsSettings settings) : base(settings)
        {
        }

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public CopyStep([NotNull] HasDestFsSettings settings, ILog logger) : base(settings, logger)
        {
        }

        public override void Apply()
        {
            Logger.Debug($"Copy {Settings.Src} to {Settings.Dest}");
            if (Settings.SrcIsDirectory)
            {
                Logger.Debug("Src is directory. Copy recursive");
                FsHelper.CopyDirectory(Settings.SrcInfo as DirectoryInfo, new DirectoryInfo(Settings.Dest));
            }
            else
            {
                Logger.Debug("Src is file");
                (Settings.SrcInfo as FileInfo).CopyTo(Settings.Dest);
            }
        }

        public override void Rollback()
        {
            Logger.Info($"Rollback copy {Settings.Src}. Remove {Settings.Dest}");
            if (Settings.SrcIsDirectory)
            {
                Logger.Debug("Src is directory. Remove recursive");
                Directory.Delete(Settings.Dest, true);
            }
            else
            {
                Logger.Debug("Src is file");
                File.Delete(Settings.Dest);
            }
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string src, [NotNull] string dest)
        {
            return new CopyStep(new HasDestFsSettings(name, src, dest));
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string src, [NotNull] string dest, ILog logger)
        {
            return new CopyStep(new HasDestFsSettings(name, src, dest), logger);
        }
    }
}