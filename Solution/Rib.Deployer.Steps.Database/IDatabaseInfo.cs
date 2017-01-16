namespace Rib.Deployer.Steps.Database
{
    using System;
    using JetBrains.Annotations;

    public interface IDatabaseInfo : IDisposable
    {
        /// <summary>Имя базы данных</summary>
        string Name { get; }

        /// <summary>Создать пустую бд</summary>
        void Create();

        /// <summary>Удалить базу данных</summary>
        void Drop();

        /// <summary>Бд существует на сервере</summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>Забекапить</summary>
        /// <param name="backupPath"></param>
        void Backup([NotNull] string backupPath);

        /// <summary>Восстановить</summary>
        /// <param name="backupPath"></param>
        void Restore([NotNull] string backupPath);
    }
}