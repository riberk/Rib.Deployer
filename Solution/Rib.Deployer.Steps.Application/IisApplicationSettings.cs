namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.ComponentModel;
    using JetBrains.Annotations;

    public class IisApplicationSettings : IStepSettings
    {
        public enum State
        {
            Start = 1,
            Stop = 2
        }

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public IisApplicationSettings([NotNull] string name, [NotNull] string site, State newState)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(site)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(site));
            if (!Enum.IsDefined(typeof(State), newState))
                throw new InvalidEnumArgumentException(nameof(newState), (int) newState, typeof(State));
            Name = name;
            Site = site;
            NewState = newState;
        }

        [NotNull]
        public string Site { get; }

        public State NewState { get; }

        public string Name { get; }
    }
}