namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using Common.Logging;
    using JetBrains.Annotations;

    public class RestoreStep : DeployStepBase<RestoreSettings>, IDisposable
    {
        [NotNull] private readonly IDatabaseInfo _databaseInfo;
        private readonly bool _infoOwner;


        public RestoreStep([NotNull] RestoreSettings settings,
                           [NotNull] IDatabaseInfo databaseInfo,
                           bool infoOwner = false) : this(settings, databaseInfo, infoOwner, null)
        {
        }

        public RestoreStep([NotNull] RestoreSettings settings,
                           [NotNull] IDatabaseInfo databaseInfo,
                           bool infoOwner,
                           ILog logger) : base(settings, logger)
        {
            if (databaseInfo == null) throw new ArgumentNullException(nameof(databaseInfo));
            _databaseInfo = databaseInfo;
            _infoOwner = infoOwner;
        }

        protected virtual PreviousState PrevState { get; set; }

        public void Dispose()
        {
            if (PrevState.Exists)
            {
                File.Delete(PrevState.BackupPath);
            }
            if (_infoOwner)
            {
                _databaseInfo.Dispose();
            }
        }

        public override void Apply()
        {
            var exists = _databaseInfo.Exists();
            if (exists)
            {
                Logger.Info($"Database {_databaseInfo.Name} exists. Backup");
                var backupPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bak");
                _databaseInfo.Backup(backupPath, Settings.CommandTimeout);
                PrevState = new PreviousState(backupPath, true);
            }
            else
            {
                PrevState = new PreviousState(null, false);
            }

            Logger.Info($"Restoring database {_databaseInfo.Name}");
            _databaseInfo.Restore(Settings.BackupPath, Settings.CommandTimeout);
        }

        public override void Rollback()
        {
            if (PrevState.Exists)
            {
                _databaseInfo.Restore(PrevState.BackupPath, Settings.CommandTimeout);
            }
            else
            {
                _databaseInfo.Drop();
            }
        }

        public static IDeployStep Create(string name, string connectionString, string backupPath, int commandTimeout)
        {
            return Create(name, connectionString, backupPath, commandTimeout, null);
        }

        public static IDeployStep Create(string name, string connectionString, string backupPath, int commandTimeout, ILog logger)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = sqlConnectionStringBuilder.InitialCatalog;
            sqlConnectionStringBuilder.InitialCatalog = "master";
            var info = new DatabaseInfo(databaseName, sqlConnectionStringBuilder.ToString(), false);
            return new RestoreStep(new RestoreSettings(name, backupPath) {CommandTimeout = commandTimeout}, info, true, logger);
        }

        public class PreviousState
        {
            public PreviousState(string backupPath, bool exists)
            {
                BackupPath = backupPath;
                Exists = exists;
            }

            public string BackupPath { get; }

            public bool Exists { get; }
        }
    }
}