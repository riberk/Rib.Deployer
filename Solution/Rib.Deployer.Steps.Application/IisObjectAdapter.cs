namespace Rib.Deployer.Steps.Application
{
    using System;
    using Microsoft.Web.Administration;

    internal abstract class IisObjectAdapter : IIisObject
    {
        public abstract IisObjectState Start();
        public abstract IisObjectState State { get; }
        public abstract IisObjectState Stop();
        public abstract string Name { get; }

        public IisObjectState SetState(IisObjectState state)
        {
            switch (state)
            {
                case IisObjectState.Started:
                    return Start();
                case IisObjectState.Stoped:
                    return Stop();
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public static IisObjectState MapState(ObjectState state)
        {
            switch (state)
            {
                case ObjectState.Starting:
                case ObjectState.Stopping:
                case ObjectState.Unknown:
                    throw new NotSupportedException($"Only {ObjectState.Started} and {ObjectState.Stopped} supported");
                case ObjectState.Started:
                    return IisObjectState.Started;
                case ObjectState.Stopped:
                    return IisObjectState.Stoped;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}