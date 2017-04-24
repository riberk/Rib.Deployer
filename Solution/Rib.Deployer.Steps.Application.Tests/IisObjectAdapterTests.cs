namespace Rib.Deployer.Steps.Application
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Administration;

    [TestClass]
    public class IisObjectAdapterTests
    {
        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetStateThrows()
        {
            var a = new Adapter(IisObjectState.Started, "name");
            a.SetState((IisObjectState) 100);
        }


        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MapState()
        {
            IisObjectAdapter.MapState(ObjectState.Starting);
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