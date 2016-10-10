namespace Rib.Deployer.Steps.Application
{
    using Microsoft.Web.Administration;

    internal interface IIIsObject
    {
        ObjectState Start();

        ObjectState State { get;  }

        ObjectState Stop();
        string Name { get; }
    }
}