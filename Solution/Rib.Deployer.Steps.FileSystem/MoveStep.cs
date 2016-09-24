namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using JetBrains.Annotations;

    public class MoveStep : DeployStepBase<HasDestFsSettings>
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.Object" />.</summary>
        public MoveStep([NotNull] HasDestFsSettings settings) : base(settings)
        {
        }

        public override void Apply()
        {
            Logger.Debug($"Move {Settings.Src} to {Settings.Dest}");
            if (Settings.SrcIsDirectory)
            {
                Logger.Debug("Src is directory");
                (Settings.SrcInfo as DirectoryInfo).MoveTo(Settings.Dest);
            }
            else
            {
                Logger.Debug("Src is file");
                (Settings.SrcInfo as FileInfo).MoveTo(Settings.Dest);
            }
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
            return new MoveStep(new HasDestFsSettings(name, src, dest));
        }
    }
}