namespace Rib.Deployer.Steps
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public class TuppleSettings : IStepSettings
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.Object" />.</summary>
        public TuppleSettings([NotNull] string name, [NotNull] [ItemNotNull] params IDeployStep[] steps)
        {
            if (steps == null) throw new ArgumentNullException(nameof(steps));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (steps.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(steps));

            Name = name;
            Steps = steps;
        }

        public string Name { get; }

        [NotNull, ItemNotNull]
        public IReadOnlyCollection<IDeployStep> Steps { get; }
    }
}