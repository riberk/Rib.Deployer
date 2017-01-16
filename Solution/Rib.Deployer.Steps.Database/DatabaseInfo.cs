namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using JetBrains.Annotations;

    public class DatabaseInfo : IDatabaseInfo
    {
        [NotNull]
        private readonly SqlConnection _connection;

        private readonly bool _isDatabaseOwner;

        private bool _disposed;

        public DatabaseInfo([NotNull] string databaseName, [NotNull] string masterConnectionString, bool isDatabaseOwner)
        {
            if (databaseName == null) throw new ArgumentNullException(nameof(databaseName));
            if (string.IsNullOrWhiteSpace(masterConnectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(masterConnectionString));
            Name = databaseName;
            _isDatabaseOwner = isDatabaseOwner;
            _connection = new SqlConnection(masterConnectionString);
            _connection.Open();
        }
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            if (_isDatabaseOwner)
            {
                Drop();
            }
            _connection.Dispose();
            _disposed = true;
        }

        public void Drop()
        {
            using (var cmd = new SqlCommand
            {
                Connection = _connection
            })
            {
                DropInternal(cmd);
            }
        }

       public bool Exists()
        {
            var cmdText = $"SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = N'{Name}' OR name = N'{Name}')";
            using (var cmd = new SqlCommand(cmdText, _connection))
            {
                var value = (string)cmd.ExecuteScalar();
                return !string.IsNullOrWhiteSpace(value) && value.Equals(Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        public void Backup(string backupPath)
        {
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(backupPath));
            using (var cmd = new SqlCommand($"backup database [{Name}] to disk = N'{backupPath}'", _connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void Restore(string backupPath)
        {
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(backupPath));
            using (var cmd = new SqlCommand
            {
                Connection = _connection
            })
            {
                if (Exists())
                {
                    cmd.CommandText = $"alter database [{Name}] set SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"Use [{Name}];";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "Use [master];";
                    cmd.ExecuteNonQuery();
                }


                cmd.CommandText = $"restore database [{Name}] from disk = N'{backupPath}'";
                cmd.ExecuteNonQuery();
            }
        }

        public void Create()
        {
            using (var cmd = new SqlCommand($"CREATE DATABASE [{Name}]", _connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void DropInternal([NotNull] IDbCommand command)
        {
            command.CommandText = $"alter database [{Name}] set SINGLE_USER WITH ROLLBACK IMMEDIATE";
            command.ExecuteNonQuery();

            command.CommandText = $"Use [{Name}];";
            command.ExecuteNonQuery();

            command.CommandText = "Use [master];";
            command.ExecuteNonQuery();

            command.CommandText = $"EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{Name}'";
            command.ExecuteNonQuery();

            command.CommandText = $"DROP DATABASE [{Name}]";
            command.ExecuteNonQuery();
        }

        [NotNull]
        public string Name { get; }
    }
}