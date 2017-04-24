namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.ComponentModel;
    using JetBrains.Annotations;

    public class IisApplicationSettings : IStepSettings
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public IisApplicationSettings([NotNull] string name,
                                      [NotNull] string objectName,
                                      IisObjectState newState,
                                      int waitDuration,
                                      int maxWaits)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(objectName));
            if (!Enum.IsDefined(typeof(IisObjectState), newState))
                throw new InvalidEnumArgumentException(nameof(newState), (int) newState, typeof(IisObjectState));
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

        public IisObjectState NewState { get; }

        /// <summary>Длительность одного ожидания перехода в новый стату</summary>
        public int WaitDuration { get; }

        /// <summary>Максимальное количество ожиданий</summary>
        public int MaxWaits { get; }

        public string Name { get; }

        //int waitDuration = 1000, int maxWaits = 5
    }
}