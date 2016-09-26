namespace Rib.Deployer.Steps
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public class TuppleSettings : IStepSettings
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.Object" />.</summary>
        public TuppleSettings([NotNull] string name, [NotNull, ItemNotNull] params IDeployStep[] steps)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (steps == null) throw new ArgumentNullException(nameof(steps));
            Name = name;
            Steps = steps;
        }

        public string Name { get; }

        [NotNull, ItemNotNull]
        public IReadOnlyCollection<IDeployStep> Steps { get; }
    }
}