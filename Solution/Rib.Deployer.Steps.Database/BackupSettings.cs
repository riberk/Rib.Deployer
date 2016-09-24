namespace Rib.Deployer.Steps.Database
{
    using System;

    public class BackupSettings : IStepSettings
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public BackupSettings(string name, string connectionString, string backupPath)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(backupPath));
            Name = name;
            ConnectionString = connectionString;
            BackupPath = backupPath;
        }

        public string ConnectionString { get; }

        public string BackupPath { get; }

        public string Name { get; }
    }
}