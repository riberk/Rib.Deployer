namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using System.IO;
    using JetBrains.Annotations;

    public class FsSettings : IStepSettings
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public FsSettings([NotNull] string name, [NotNull] string src)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(src)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(src));
            Name = name;
            Src = src;
        }

        [NotNull]
        public string Src { get; }

        public string Name { get; }

        public bool SrcIsDirectory => (bool)(_isDirectory ?? (_isDirectory = GetIsDirectory()));

        private bool GetIsDirectory() => (File.GetAttributes(Src) & FileAttributes.Directory) == FileAttributes.Directory;

        [NotNull]
        public FileSystemInfo SrcInfo => SrcIsDirectory ? (FileSystemInfo)new DirectoryInfo(Src) : new FileInfo(Src);

        private bool? _isDirectory;
    }
}