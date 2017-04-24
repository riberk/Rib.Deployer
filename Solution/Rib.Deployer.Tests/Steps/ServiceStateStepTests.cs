namespace Rib.Deployer.Steps
{
    using System;
    using System.ServiceProcess;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceStateStepTests
    {
        private const string ServiceName = "WSearch";

        [TestMethod]
        public void ApplyStartTest()
        {
            var starter = ServiceStateStep.CreateStarter("Test", ServiceName, null);
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
                Assert.AreEqual(ServiceControllerStatus.Stopped, sc.Status);
                starter.Apply();
                sc.Refresh();
                Assert.AreEqual(ServiceControllerStatus.Running, sc.Status);
            }
        }

        [TestMethod]
        public void ApplyStopTest()
        {
            var starter = ServiceStateStep.CreateStoper("Test", ServiceName, null);
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                Assert.AreEqual(ServiceControllerStatus.Running, sc.Status);
                starter.Apply();
                sc.Refresh();
                Assert.AreEqual(ServiceControllerStatus.Stopped, sc.Status);
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            }
        }

        [TestMethod]
        public void RollbackBeforeStateNotEq()
        {
            var starter = (ServiceStateStep)ServiceStateStep.CreateStarter("Test", ServiceName, null);
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                Assert.AreEqual(ServiceControllerStatus.Running, sc.Status);
#pragma warning disable CS0618 // Тип или член устарел
                starter.SetBeforeApplyState(WindowsServiceState.Stoped);
#pragma warning restore CS0618 // Тип или член устарел
                starter.Rollback();
                sc.Refresh();
                Assert.AreEqual(ServiceControllerStatus.Stopped, sc.Status);
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            }
        }

        [TestMethod]
        public void RollbackBeforeStateEq()
        {
            var starter = (ServiceStateStep)ServiceStateStep.CreateStarter("Test", ServiceName, null);
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                Assert.AreEqual(ServiceControllerStatus.Running, sc.Status);
#pragma warning disable CS0618 // Тип или член устарел
                starter.SetBeforeApplyState(WindowsServiceState.Started);
#pragma warning restore CS0618 // Тип или член устарел
                starter.Rollback();
                sc.Refresh();
                Assert.AreEqual(ServiceControllerStatus.Running, sc.Status);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UndefinedStateTest()
        {
            var starter = new ServiceStateStep(new ServiceStateSettings("1", ServiceName, (WindowsServiceState) 100, 100), null);
            starter.Apply();
        }
    }
}