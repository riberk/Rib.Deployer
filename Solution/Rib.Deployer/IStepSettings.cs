namespace Rib.Deployer
{
    using JetBrains.Annotations;

    public interface IStepSettings
    {
        /// <summary>Имя шага</summary>
        [NotNull]
        string Name { get; }
    }
}