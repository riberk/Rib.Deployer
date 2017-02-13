namespace Rib.Deployer.Steps.Database
{
    using System;
    using JetBrains.Annotations;

    public class BackupSettings : IStepSettings
    {
        public const int DefaultCommandTimeout = 30;

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public BackupSettings(string name, string backupPath)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(backupPath));
            Name = name;
            BackupPath = backupPath;
        }

        [NotNull]
        public string BackupPath { get; }

        public int CommandTimeout { get; set; } = DefaultCommandTimeout;
        
        public string Name { get; }
    }
}