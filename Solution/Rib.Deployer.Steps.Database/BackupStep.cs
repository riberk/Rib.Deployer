namespace Rib.Deployer.Steps.Database
{
    using System.Data.SqlClient;
    using JetBrains.Annotations;

    public class BackupStep : DeployStepBase<BackupSettings>
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public BackupStep([NotNull] BackupSettings settings) : base(settings)
        {
        }

        /// <summary>Применить шаг</summary>
        public override void Apply()
        {
            var databaseName = new SqlConnectionStringBuilder(Settings.ConnectionString).InitialCatalog;
            Logger.Info($"Backup {databaseName}");
            using (var connection = new SqlConnection(Settings.ConnectionString))
            using (var cmd = new SqlCommand($"backup database [{databaseName}] to disk = N'{Settings.BackupPath}'", connection))
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Откатить шаг</summary>
        public override void Rollback()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(Settings.ConnectionString);
            var databaseName = sqlConnectionStringBuilder.InitialCatalog;
            Logger.Info($"Restore {databaseName}");
            sqlConnectionStringBuilder.InitialCatalog = "master";
            using (var connection = new SqlConnection(sqlConnectionStringBuilder.ToString()))
            using (var cmd = new SqlCommand()
            {
                Connection = connection
            })
            {
                Logger.Trace($"Restoring database {databaseName}");
                connection.Open();

                Logger.Trace("Set single user");
                cmd.CommandText = $"alter database [{databaseName}] set SINGLE_USER WITH ROLLBACK IMMEDIATE";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"Use [{databaseName}];";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Use [master];";
                cmd.ExecuteNonQuery();

                Logger.Trace("Restoring");
                cmd.CommandText = $"restore database [{databaseName}] from disk = N'{Settings.BackupPath}'";
                cmd.ExecuteNonQuery();
            }
        }

        public static IDeployStep Create(string name, string connectionString, string backupPath)
        {
            return new BackupStep(new BackupSettings(name, connectionString, backupPath));
        }
    }
}