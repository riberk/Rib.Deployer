namespace Rib.Deployer.Steps.Application
{
    using System;
    using Microsoft.Web.Administration;
    using NUnit.Framework;

    [TestFixture]
    public class IisObjectAdapterTests
    {
        [Test]
        public void SetStateTest()
        {
            var a = new Adapter(IisObjectState.Started, "name");
            Assert.AreEqual(0, a.CountStart);
            Assert.AreEqual(0, a.CountStop);

            a.SetState(IisObjectState.Started);
            Assert.AreEqual(1, a.CountStart);
            Assert.AreEqual(0, a.CountStop);

            a.SetState(IisObjectState.Stoped);
            Assert.AreEqual(1, a.CountStart);
            Assert.AreEqual(1, a.CountStop);
        }

        [Test]
        public void SetStateThrows()
        {
            var a = new Adapter(IisObjectState.Started, "name");
            Assert.Throws<ArgumentOutOfRangeException>(() => a.SetState((IisObjectState) 100));
        }


        [Test]
        public void MapState()
        {

            Assert.Throws<NotSupportedException>(() => IisObjectAdapter.MapState(ObjectState.Starting));
            Assert.Throws<NotSupportedException>(() => IisObjectAdapter.MapState(ObjectState.Stopping));
            Assert.Throws<NotSupportedException>(() => IisObjectAdapter.MapState(ObjectState.Unknown));
        }

        private class Adapter : IisObjectAdapter
        {
            private IisObjectState _state;
            public int CountStart { get; private set; }
            public int CountStop { get; private set; }

            public Adapter(IisObjectState state, string name)
            {
                _state = state;
                Name = name;
            }

            public override IisObjectState Start()
            {
                CountStart++;
                _state = IisObjectState.Started;
                return State;
            }

            public override IisObjectState State => _state;
            public override IisObjectState Stop()
            {
                CountStop++;
                _state = IisObjectState.Stoped;
                return State;
            }

            public override string Name { get; }
        }
    }
}