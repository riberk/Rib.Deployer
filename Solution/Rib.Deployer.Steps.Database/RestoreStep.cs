namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.IO;
    using JetBrains.Annotations;

    public class RestoreStep : DeployStepBase<RestoreSettings>, IDisposable
    {
        [NotNull] private readonly IDatabaseInfo _databaseInfo;


        public RestoreStep([NotNull] RestoreSettings settings,
                           [NotNull] IDatabaseInfo databaseInfo) : base(settings)
        {
            if (databaseInfo == null) throw new ArgumentNullException(nameof(databaseInfo));
            _databaseInfo = databaseInfo;
        }

        protected virtual PreviousState PrevState { get; set; }

        public void Dispose()
        {
            if (PrevState.Exists)
            {
                File.Delete(PrevState.BackupPath);
            }
        }

        public override void Apply()
        {
            var exists = _databaseInfo.Exists();
            if (exists)
            {
                Logger.Info($"Database {_databaseInfo.Name} exists. Backup");
                var backupPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bak");
                _databaseInfo.Backup(backupPath);
                PrevState = new PreviousState(backupPath, true);
            }
            else
            {
                PrevState = new PreviousState(null, false);
            }

            Logger.Info($"Restoring database {_databaseInfo.Name}");
            _databaseInfo.Restore(Settings.BackupPath);
        }

        public override void Rollback()
        {
            if (PrevState.Exists)
            {
                _databaseInfo.Restore(PrevState.BackupPath);
            }
            else
            {
                _databaseInfo.Drop();
            }
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