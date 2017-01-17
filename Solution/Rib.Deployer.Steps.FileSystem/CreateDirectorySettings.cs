namespace Rib.Deployer.Steps.FileSystem
{
    using System;
    using JetBrains.Annotations;

    public class CreateDirectorySettings : IStepSettings
    {
        public CreateDirectorySettings([NotNull] string path,
                                       [NotNull] string name, 
                                       bool recursiveRemoveOnRollback)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            Path = path;
            Name = name;
            RecursiveRemoveOnRollback = recursiveRemoveOnRollback;
        }

        [NotNull]
        public string Path { get; }

        public string Name { get; }

        public bool RecursiveRemoveOnRollback { get; }
    }
}