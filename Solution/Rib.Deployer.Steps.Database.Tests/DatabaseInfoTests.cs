namespace Rib.Deployer.Steps.Database
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Threading;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseInfoTests
    {
        private const string MasterConnectionString = "Data Source=CurrentServer;Initial Catalog=master;Integrated Security=True;";

        [TestMethod]
        public void DisposeOwnerTest()
        {
            const string dbName = "RibDeployer_DatabaseInfoTests_DisposeOwnerTest";
            using (new Db(dbName, MasterConnectionString))
            using (var dbInfo = new DatabaseInfo(dbName, MasterConnectionString, true))
            {
                Assert.IsTrue(dbInfo.Exists());
                dbInfo.Dispose();
                using (var notDisposableDbInfo = new DatabaseInfo(dbName, MasterConnectionString, false))
                {
                    Assert.IsFalse(notDisposableDbInfo.Exists());
                }
            }
        }

        [TestMethod]
        public void DisposeIsNotOwnerTest()
        {
            const string dbName = "RibDeployer_DatabaseInfoTests_DisposeIsNotOwnerTest";
            using (new Db(dbName, MasterConnectionString))
            using (var dbInfo = new DatabaseInfo(dbName, MasterConnectionString, false))
            {
                Assert.IsTrue(dbInfo.Exists());
                dbInfo.Dispose();
                using (var notDisposableDbInfo = new DatabaseInfo(dbName, MasterConnectionString, false))
                {
                    Assert.IsTrue(notDisposableDbInfo.Exists());
                }
            }
        }

        [TestMethod]
        public void DropTest()
        {
            const string dbName = "RibDeployer_DatabaseInfoTests_DropTest";
            using (new Db(dbName, MasterConnectionString))
            using (var dbInfo = new DatabaseInfo(dbName, MasterConnectionString, false))
            {
                Assert.IsTrue(dbInfo.Exists());
                dbInfo.Drop();
                Assert.IsFalse(dbInfo.Exists());
            }
        }

        [TestMethod]
        public void ExistsTest()
        {
            const string dbName = "RibDeployer_DatabaseInfoTests_ExistsTest";
            using (var dbInfo = new DatabaseInfo(dbName, MasterConnectionString, false))
            {
                Assert.IsFalse(dbInfo.Exists());
                using (new Db(dbName, MasterConnectionString))
                {
                    Assert.IsTrue(dbInfo.Exists());
                }
                Assert.IsFalse(dbInfo.Exists());
            }
        }

        [TestMethod]
        public void BackupTest()
        {
            const string dbName = "RibDeployer_DatabaseInfoTests_BackupTest";
            var backupPath = Path.Combine(Directory.GetCurrentDirectory(), $"{dbName}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");

            using (var db = new Db(dbName, MasterConnectionString))
            using (var dbInfo = new DatabaseInfo(dbName, MasterConnectionString, false))
            {
                dbInfo.Backup(backupPath, 10);
                Assert.IsTrue(File.Exists(backupPath));
                db.EnsureBakcupIsValid(backupPath);
            }
        }

        [TestMethod]
        public void RestoreWithoutDatabaseTest()
        {
            const string databaseName = "RibDeployer_DatabaseInfoTests_RestoreWithoutDatabaseTest";
            var backupPath = Path.Combine(Directory.GetCurrentDirectory(), $"{databaseName}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");
            using (var db = new Db(databaseName, MasterConnectionString))
            using (var dbInfo = new DatabaseInfo(databaseName, MasterConnectionString, false))
            {
                dbInfo.Backup(backupPath, 10);
                Assert.IsTrue(dbInfo.Exists());
                dbInfo.Drop();
                Assert.IsFalse(dbInfo.Exists());

                var fromDt = DateTime.Now;
                Thread.Sleep(20);
                dbInfo.Restore(backupPath, 10);


                var lastRestore = db.LastRestore();
                Assert.IsNotNull(lastRestore);
                Assert.IsTrue(lastRestore > fromDt,
                              $"Last restored database at {lastRestore.Value:dd.MM.yyyy HH:mm:ss ms} but start rollback at {fromDt:dd.MM.yyyy HH:mm:ss ms}");
                Assert.IsTrue(dbInfo.Exists());
            }
        }

        [TestMethod]
        public void RestoreWithDatabaseTest()
        {
            const string databaseName = "RibDeployer_DatabaseInfoTests_RestoreWithDatabaseTest";
            var backupPath = Path.Combine(Directory.GetCurrentDirectory(), $"{databaseName}.{DateTime.Now:yyyy-MM-ddTHHmmss}.bak");
            using (var db = new Db(databaseName, MasterConnectionString))
            using (var dbInfo = new DatabaseInfo(databaseName, MasterConnectionString, false))
            {
                dbInfo.Backup(backupPath, 10);
                Assert.IsTrue(dbInfo.Exists());

                var fromDt = DateTime.Now;
                Thread.Sleep(20);

                dbInfo.Restore(backupPath, 10);

                var lastRestore = db.LastRestore();
                Assert.IsNotNull(lastRestore);
                Assert.IsTrue(lastRestore > fromDt,
                              $"Last restored database at {lastRestore.Value:dd.MM.yyyy HH:mm:ss ms} but start rollback at {fromDt:dd.MM.yyyy HH:mm:ss ms}");
                Assert.IsTrue(dbInfo.Exists());
            }
        }

        [TestMethod]
        public void CreateTest()
        {
            const string dbName = "RibDeployer_DatabaseInfoTests_CreateTest";
            using (var dbInfo = new DatabaseInfo(dbName, MasterConnectionString, true))
            {
                Assert.IsFalse(dbInfo.Exists());
                dbInfo.Create();
                Assert.IsTrue(dbInfo.Exists());
            }
        }

        private class Db : IDisposable
        {
            [NotNull] private readonly string _databaseName;

            [NotNull] private readonly string _masterConnectionString;

            public Db([NotNull] string databaseName, [NotNull] string masterConnectionString)
            {
                if (string.IsNullOrWhiteSpace(databaseName))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(databaseName));
                if (string.IsNullOrWhiteSpace(masterConnectionString))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(masterConnectionString));
                _databaseName = databaseName;
                _masterConnectionString = masterConnectionString;
                using (var connection = new SqlConnection(masterConnectionString))
                using (var cmd = new SqlCommand($"CREATE DATABASE [{_databaseName}]", connection))
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public void Dispose()
            {
                using (var connection = new SqlConnection(_masterConnectionString))
                using (var command = new SqlCommand
                {
                    Connection = connection
                })
                {
                    connection.Open();
                    command.CommandText =
                            $"SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = N'{_databaseName}' OR name = N'{_databaseName}')";
                    var value = (string) command.ExecuteScalar();
                    var exists = !string.IsNullOrWhiteSpace(value) && value.Equals(_databaseName, StringComparison.OrdinalIgnoreCase);

                    if (!exists)
                    {
                        return;
                    }

                    command.CommandText = $"alter database [{_databaseName}] set SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    command.ExecuteNonQuery();

                    command.CommandText = $"Use [{_databaseName}];";
                    command.ExecuteNonQuery();

                    command.CommandText = "Use [master];";
                    command.ExecuteNonQuery();

                    command.CommandText = $"EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{_databaseName}'";
                    command.ExecuteNonQuery();

                    command.CommandText = $"DROP DATABASE [{_databaseName}]";
                    command.ExecuteNonQuery();
                }
            }

            public void EnsureBakcupIsValid(string path)
            {
                using (var connection = new SqlConnection(_masterConnectionString))
                using (var cmd = new SqlCommand($"RESTORE VERIFYONLY FROM DISK = N'{path}'", connection))
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
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
                            WHERE [RowNum] = 1 and DatabaseName = '{_databaseName}'";
                using (var connection = new SqlConnection(_masterConnectionString))
                using (var cmd = new SqlCommand(sql, connection))
                {
                    connection.Open();
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
        }
    }
}