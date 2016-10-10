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
        public IisApplicationSettings([NotNull] string name,
                                      [NotNull] string objectName,
                                      State newState,
                                      int waitDuration,
                                      int maxWaits)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(objectName));
            if (!Enum.IsDefined(typeof(State), newState))
                throw new InvalidEnumArgumentException(nameof(newState), (int) newState, typeof(State));
            if (waitDuration <= 0) throw new ArgumentOutOfRangeException(nameof(waitDuration));
            if (maxWaits < 0) throw new ArgumentOutOfRangeException(nameof(maxWaits));
            Name = name;
            ObjectName = objectName;
            NewState = newState;
            WaitDuration = waitDuration;
            MaxWaits = maxWaits;
        }

        [NotNull]
        public string ObjectName { get; }

        public State NewState { get; }

        /// <summary>Длительность одного ожидания перехода в новый стату</summary>
        public int WaitDuration { get; }

        /// <summary>Максимальное количество ожиданий</summary>
        public int MaxWaits { get; }

        public string Name { get; }

        //int waitDuration = 1000, int maxWaits = 5
    }
}