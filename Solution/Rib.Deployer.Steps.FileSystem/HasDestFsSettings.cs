namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using JetBrains.Annotations;

    public class HasDestFsSettings : FsSettings
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public HasDestFsSettings([NotNull] string name, [NotNull] string src, [NotNull] string dest) : base(name, src)
        {
            if (string.IsNullOrWhiteSpace(dest)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(dest));
            Dest = dest;
        }

        [NotNull]
        public string Dest { get; }
    }
}