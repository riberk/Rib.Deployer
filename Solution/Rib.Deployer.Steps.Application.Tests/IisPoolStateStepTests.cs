namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Administration;

    [TestClass]
    public class IisPoolStateStepTests
    {
        private static readonly object LockObj = new object();

        [TestMethod]
        public void ApplyStopTest()
        {
            const string poolName = "ApplyStopPool";
            using (var s = CreatePool(poolName))
            {
                s.EnsureStart();
                Assert.AreEqual(ObjectState.Started, s.State());
                var step = new IisPoolStateStep(new IisApplicationSettings("stop pool", poolName, IisObjectState.Stoped, 1000, 10), null);
                step.Apply();
                Assert.AreEqual(ObjectState.Stopped, s.State());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ApplyWithoutSiteTest()
        {
            const string poolName = "ApplyWithoutPoolTest";
            var step = new IisPoolStateStep(new IisApplicationSettings("stop pool", poolName, IisObjectState.Stoped, 1000, 10), null);
            step.Apply();
        }

        [TestMethod]
        public void RollbackWithoutSiteTest()
        {
            const string poolName = "RollbackWithoutPoolTest";
            var step = new IisPoolStateStep(new IisApplicationSettings("stop pool", poolName, IisObjectState.Stoped, 1000, 10), null);
            step.Rollback();
        }

        [TestMethod]
        public void ApplyStartTest()
        {
            const string poolName = "ApplyStartPool";
            using (var pool = CreatePool(poolName))
            {
                pool.EnsureStop();
                Assert.AreEqual(ObjectState.Stopped, pool.State());
                var step = new IisPoolStateStep(new IisApplicationSettings("start pool", poolName, IisObjectState.Started, 1000, 10), null);
                step.Apply();
                Assert.AreEqual(ObjectState.Started, pool.State());
            }
        }

        [TestMethod]
        public void RollbackStopTest()
        {
            const string poolName = "RollbackStopPool";
            using (var pool = CreatePool(poolName))
            {
                pool.EnsureStart();
                Assert.AreEqual(ObjectState.Started, pool.State());
                var step = new IisPoolStateStep(new IisApplicationSettings("Rollback pool", poolName, IisObjectState.Stoped, 1000, 10), null);
                step.Apply();
                Assert.AreEqual(ObjectState.Stopped, pool.State());
                step.Rollback();
                Assert.AreEqual(ObjectState.Started, pool.State());
            }
        }

        [TestMethod]
        public void RollbackStartTest()
        {
            const string poolName = "RollbackStartPool";
            using (var pool = CreatePool(poolName))
            {
                pool.EnsureStop();
                Assert.AreEqual(ObjectState.Stopped, pool.State());
                var step = new IisPoolStateStep(new IisApplicationSettings("Rollback pool", poolName, IisObjectState.Started, 1000, 10), null);
                step.Apply();
                Assert.AreEqual(ObjectState.Started, pool.State());
                step.Rollback();
                Assert.AreEqual(ObjectState.Stopped, pool.State());
            }
        }


        [NotNull]
        private TempPool CreatePool([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            using (var sm = new ServerManager())
            {
                var pool = sm.ApplicationPools.Add(name);
                pool.AutoStart = true;
                sm.CommitChanges();
            }
            //TODO Ќе успевает применитьс€ и site.State далее падает с COM-exception.
            // 50 установлено методом подбора. ѕерестает падать где-то на 7, 50 - на вс€кий случай
            Thread.Sleep(50);
            return new TempPool(name);
        }

        private class TempPool : IDisposable
        {
            [NotNull] private readonly string _poolName;

            public TempPool([NotNull] string poolName)
            {
                if (poolName == null) throw new ArgumentNullException(nameof(poolName));
                _poolName = poolName;
            }

            /// <summary>
            ///     ¬ыполн€ет определ€емые приложением задачи, св€занные с удалением, высвобождением или сбросом неуправл€емых
            ///     ресурсов.
            /// </summary>
            public void Dispose()
            {
                lock (LockObj)
                {
                    using (var serverManager = new ServerManager())
                    {
                        var pool = serverManager.ApplicationPools[_poolName];
                        if (pool != null)
                        {
                            serverManager.ApplicationPools.Remove(pool);
                        }

                        serverManager.CommitChanges();
                    }
                }
            }

            public void EnsureStart()
            {
                using (var sm = new ServerManager())
                {
                    var pool = sm.ApplicationPools[_poolName];
                    if (pool.State != ObjectState.Started)
                    {
                        pool.Start();
                    }
                    if (pool.State != ObjectState.Started)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public void EnsureStop()
            {
                using (var sm = new ServerManager())
                {
                    var pool = sm.ApplicationPools[_poolName];
                    if (pool.State != ObjectState.Stopped)
                    {
                        pool.Stop();
                    }
                    if (pool.State != ObjectState.Stopped)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public ObjectState State()
            {
                using (var sm = new ServerManager())
                {
                    var pool = sm.ApplicationPools[_poolName];
                    return pool.State;
                }
            }
        }
    }
}