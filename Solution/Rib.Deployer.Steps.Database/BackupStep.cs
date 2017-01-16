namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.Data.SqlClient;
    using Common.Logging;
    using JetBrains.Annotations;

    public class BackupStep : DeployStepBase<BackupSettings>, IDisposable
    {
        [NotNull] private readonly IDatabaseInfo _databaseInfo;
        private readonly bool _infoOwner;

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public BackupStep(
            [NotNull] BackupSettings settings, 
            [NotNull] IDatabaseInfo databaseInfo,
            bool infoOwner = false) : this(settings, databaseInfo, infoOwner, null)
        {
        }

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public BackupStep(
            [NotNull] BackupSettings settings,
            [NotNull] IDatabaseInfo databaseInfo,
            bool infoOwner,
            ILog logger) : base(settings, logger)
        {
            if (databaseInfo == null) throw new ArgumentNullException(nameof(databaseInfo));
            _databaseInfo = databaseInfo;
            _infoOwner = infoOwner;
        }

        public void Dispose()
        {
            if (_infoOwner)
            {
                _databaseInfo.Dispose();
            }
        }

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            _databaseInfo.Backup(Settings.BackupPath);
        }

        /// <summary>Откатить шаг</summary>
        public override void Rollback()
        {
            _databaseInfo.Restore(Settings.BackupPath);
        }

        public static IDeployStep Create(string name, string connectionString, string backupPath)
        {
            return Create(name, connectionString, backupPath, null);
        }

        public static IDeployStep Create(string name, string connectionString, string backupPath, ILog logger)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = sqlConnectionStringBuilder.InitialCatalog;
            sqlConnectionStringBuilder.InitialCatalog = "master";
            var info = new DatabaseInfo(databaseName, sqlConnectionStringBuilder.ToString(), false);
            return new BackupStep(new BackupSettings(name, backupPath), info, true, logger);
        }
    }
}