namespace Rib.Deployer.Steps.Database
{
    using System;
    using JetBrains.Annotations;

    public class RestoreSettings : IStepSettings
    {
        public const int DefaultCommandTimeout = 30;

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public RestoreSettings([NotNull] string name, [NotNull] string backupPath)
        {
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(backupPath));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            BackupPath = backupPath;
            Name = name;
        }

        /// <summary>Путь до файла с бекапом</summary>
        [NotNull]
        public string BackupPath { get; }

        public int CommandTimeout { get; set; } = DefaultCommandTimeout;

        public string Name { get; }
    }
}