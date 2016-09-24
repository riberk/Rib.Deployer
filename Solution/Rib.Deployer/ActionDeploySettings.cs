namespace Rib.Deployer
{
    using System;
    using JetBrains.Annotations;

    public class ActionDeploySettings : IStepSettings
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public ActionDeploySettings([NotNull] string name, [NotNull] Action apply, [NotNull] Action rollback, Action close = null)
        {
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            if (rollback == null) throw new ArgumentNullException(nameof(rollback));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            Name = name;
            Apply = apply;
            Rollback = rollback;
            Close = close;
        }

        [NotNull]
        public Action Apply { get; }

        [NotNull]
        public Action Rollback { get; }

        [CanBeNull]
        public Action Close { get; }

        public string Name { get; }
    }
}