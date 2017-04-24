namespace Rib.Deployer.Steps.Application
{
    internal interface IIisObject
    {
        IisObjectState State { get; }

        string Name { get; }

        IisObjectState SetState(IisObjectState state);
    }
}