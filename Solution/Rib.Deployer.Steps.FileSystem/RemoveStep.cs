namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using System.IO;
    using JetBrains.Annotations;

    public class RemoveStep : DeployStepBase<FsSettings>, IDisposable
    {
        [NotNull] private string _tmpDirPath;
        [NotNull] private string _destPath;

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public RemoveStep([NotNull] FsSettings settings) : base(settings)
        {
        }

        public override void Apply()
        {
            Logger.Info($"Remove {Settings.Src}");
            _tmpDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _destPath = Path.Combine(_tmpDirPath, Settings.SrcInfo.Name);
            if (!Directory.Exists(_tmpDirPath))
            {
                Logger.Debug($"Create tmp dir {_tmpDirPath}");
                Directory.CreateDirectory(_tmpDirPath);
            }
            Logger.Debug($"Move {Settings.Src} to {_destPath}");
            if (Settings.SrcIsDirectory)
            {
                Logger.Debug("Src is directory");
                (Settings.SrcInfo as DirectoryInfo).MoveTo(_destPath);
            }
            else
            {
                Logger.Debug("Src is file");
                (Settings.SrcInfo as FileInfo).MoveTo(_destPath);
            }
        }

        public override void Rollback()
        {
            Logger.Info($"Rollback remove {Settings.Src}");
            Logger.Debug($"Move {_destPath} to {Settings.Src}");
            if (Settings.SrcIsDirectory)
            {
                Logger.Debug("Src is directory");
                Directory.Move(_destPath, Settings.Src);
            }
            else
            {
                Logger.Debug("Src is file");
                File.Move(_destPath, Settings.Src);
            }
        }

        /// <summary>
        ///     Финализировать шаг. Вызывается после применения всех шагов
        /// </summary>
        public void Dispose()
        {
            Logger.Info($"Remove {Settings.Src} permanent");
            Logger.Info($"Remove {_tmpDirPath}");
            Directory.Delete(_tmpDirPath, true);
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string src)
        {
            return new RemoveStep(new FsSettings(name, src));
        }
    }
}