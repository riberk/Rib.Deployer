namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackupStepTests
    {
        private const string ConnectionString = "Data Source=CurrentServer;Initial Catalog=master;Integrated Security=True;";

        [TestMethod]
        public void ApplyTest()
        {
            const string name = "RibDeployerApplyBackupStepTest";
            var builder = new SqlConnectionStringBuilder(ConnectionString) {InitialCatalog = name};
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now.ToString("yyyy-MM-ddTHHmmss")}.bak");
            using (var db = new Db(name))
            {
                new BackupStep(new BackupSettings("Apply", builder.ToString(), path)).Apply();
                Assert.IsTrue(File.Exists(path));
                db.EnsureBakcupIsValid(path);
            }
        }

        [TestMethod]
        public void RollbackWithoutDatabaseTest()
        {
            const string name = "RibDeployerRollbackWithoutDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now.ToString("yyyy-MM-ddTHHmmss")}.bak");
            var builder = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = name };
            using (var db = new Db(name))
            {
                var step = new BackupStep(new BackupSettings("Apply", builder.ToString(), path));
                step.Apply();
                Assert.IsTrue(File.Exists(path));
                db.EnsureBakcupIsValid(path);
                Assert.IsTrue(db.Exists());
                db.Drop();
                Assert.IsFalse(db.Exists());
                var fromDt = DateTime.Now;
                step.Rollback();
                var toDt = DateTime.Now;
                var ts = toDt - fromDt;

                var lastRestore = db.LastRestore();
                Assert.IsNotNull(lastRestore);
                Assert.IsTrue(ts > toDt -  lastRestore);
                Assert.IsTrue(db.Exists());
            }
        }

        [TestMethod]
        public void RollbackWithDatabaseTest()
        {
            const string name = "RibDeployerRollbackWithDatabaseTest";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{DateTime.Now.ToString("yyyy-MM-ddTHHmmss")}.bak");
            var builder = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = name };
            using (var db = new Db(name))
            {
                var step = new BackupStep(new BackupSettings("Apply", builder.ToString(), path));
                step.Apply();
                Assert.IsTrue(File.Exists(path));
                db.EnsureBakcupIsValid(path);
                Assert.IsTrue(db.Exists());
                var fromDt = DateTime.Now;
                step.Rollback();
                var toDt = DateTime.Now;
                var ts = toDt - fromDt;

                var lastRestore = db.LastRestore();
                Assert.IsNotNull(lastRestore);
                Assert.IsTrue(ts > toDt - lastRestore);
            }
        }

        [TestMethod]
        public void CreateTest()
        {
            var step = BackupStep.Create("name", "str", "path");
            Assert.IsNotNull(step as BackupStep);
            Assert.AreEqual("name", step.Name);
        }

        private class Db : IDisposable
        {
            [NotNull] private readonly SqlConnection _connection;
            [NotNull] private readonly string _name;

            public Db([NotNull] string name)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                _name = name;
                _connection = new SqlConnection(ConnectionString);
                using (var cmd = new SqlCommand($"CREATE DATABASE [{name}]", _connection))
                {
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            /// <summary>
            ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
            ///     ресурсов.
            /// </summary>
            public void Dispose()
            {
                using (var cmd = new SqlCommand
                {
                    Connection = _connection
                })
                {
                    DropInternal(cmd);
                }
                _connection.Dispose();
            }

            public void EnsureBakcupIsValid(string path)
            {
                using (var cmd = new SqlCommand($"RESTORE VERIFYONLY FROM DISK = N'{path}'", _connection))
                {
                    cmd.ExecuteNonQuery();
                }
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

            public DateTime? LastRestore()
            {
                var sql = $@"WITH LastRestores AS
                            (
                            SELECT
                                DatabaseName = [d].[name] ,
                                [d].[create_date] ,
                                [d].[compatibility_level] ,
                                [d].[collation_name] ,
                                r.*,
                                RowNum = ROW_NUMBER() OVER (PARTITION BY d.Name ORDER BY r.[restore_date] DESC)
                            FROM master.sys.databases d
                            LEFT OUTER JOIN msdb.dbo.[restorehistory] r ON r.[destination_database_name] = d.Name
                            )
                            SELECT restore_date
                            FROM [LastRestores]
                            WHERE [RowNum] = 1 and DatabaseName = '{_name}'";
                using (var cmd = new SqlCommand(sql, _connection))
                {
                    var res = cmd.ExecuteScalar();
                    if (res == null)
                    {
                        return null;
                    }
                    var dt = res as DateTime?;
                    if (dt == null)
                    {
                        throw new InvalidCastException($"Can not convert {res.GetType()} to DateTime");
                    }
                    return dt;
                }
            }

            private void DropInternal([NotNull] IDbCommand command)
            {
                command.CommandText = $"alter database [{_name}] set SINGLE_USER WITH ROLLBACK IMMEDIATE";
                command.ExecuteNonQuery();

                command.CommandText = $"Use [{_name}];";
                command.ExecuteNonQuery();

                command.CommandText = "Use [master];";
                command.ExecuteNonQuery();

                command.CommandText = $"EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{_name}'";
                command.ExecuteNonQuery();

                command.CommandText = $"DROP DATABASE [{_name}]";
                command.ExecuteNonQuery();
            }

            public bool Exists()
            {
                var cmdText = $"SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = N'{_name}' OR name = N'{_name}')";
                using (var cmd = new SqlCommand(cmdText,_connection))
                {
                    var value = (string) cmd.ExecuteScalar();
                    return !string.IsNullOrWhiteSpace(value) && value.Equals(_name, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}