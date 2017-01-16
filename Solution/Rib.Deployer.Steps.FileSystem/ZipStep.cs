namespace Rib.Deployer.Steps.FileSystem
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Common.Logging;
    using JetBrains.Annotations;

    public class ZipStep : DeployStepBase<HasDestFsSettings>
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public ZipStep([NotNull] HasDestFsSettings settings, ILog logger) : base(settings, logger)
        {
        }

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            ZipFile.CreateFromDirectory(Settings.Src, Settings.Dest, CompressionLevel.Fastest, false, Encoding.UTF8);
        }

        /// <summary>Откатить шаг</summary>
        public override void Rollback()
        {
            File.Delete(Settings.Dest);
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string src, [NotNull] string dest)
        {
            return new ZipStep(new HasDestFsSettings(name, src, dest), null);
        }

        public static IDeployStep Create([NotNull] string name, [NotNull] string src, [NotNull] string dest, ILog logger)
        {
            return new ZipStep(new HasDestFsSettings(name, src, dest), logger);
        }
    }
}