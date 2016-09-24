namespace Rib.Deployer
{
    using JetBrains.Annotations;

    public interface IStepSettings
    {
        [NotNull]
        string Name { get; }
    }
}