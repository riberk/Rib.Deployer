namespace Rib.Deployer.Steps.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Administration;

    [TestClass]
    public class IisStateStepTests
    {
        private static readonly object LockObj = new object();

        [TestMethod]
        public void ApplyStopTest()
        {
            var siteName = "ApplyStopSite";
            using (var s = CreateSite(siteName))
            {
                s.EnsureStart();
                Assert.AreEqual(ObjectState.Started, s.State());
                var step = new IisStateStep(new IisApplicationSettings("stop site", siteName, IisApplicationSettings.State.Stop));
                step.Apply();
                Assert.AreEqual(ObjectState.Stopped, s.State());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ApplyWithoutSiteTest()
        {
            var siteName = "ApplyWithoutSiteTest";
            var step = new IisStateStep(new IisApplicationSettings("stop site", siteName, IisApplicationSettings.State.Stop));
            step.Apply();
        }

        [TestMethod]
        public void RollbackWithoutSiteTest()
        {
            var siteName = "RollbackWithoutSiteTest";
            var step = new IisStateStep(new IisApplicationSettings("stop site", siteName, IisApplicationSettings.State.Stop));
            step.Rollback();
        }

        [TestMethod]
        public void ApplyStartTest()
        {
            var siteName = "ApplyStartSite";
            using (var s = CreateSite(siteName))
            {
                s.EnsureStop();
                Assert.AreEqual(ObjectState.Stopped, s.State());
                var step = new IisStateStep(new IisApplicationSettings("stop site", siteName, IisApplicationSettings.State.Start));
                step.Apply();
                Assert.AreEqual(ObjectState.Started, s.State());
            }
        }

        [TestMethod]
        public void RollbackStopTest()
        {
            var siteName = "RollbackStopSite";
            using (var s = CreateSite(siteName))
            {
                s.EnsureStart();
                Assert.AreEqual(ObjectState.Started, s.State());
                var step = new IisStateStep(new IisApplicationSettings("stop site", siteName, IisApplicationSettings.State.Stop));
                step.Apply();
                Assert.AreEqual(ObjectState.Stopped, s.State());
                step.Rollback();
                Assert.AreEqual(ObjectState.Started, s.State());
            }
        }

        [TestMethod]
        public void RollbackStartTest()
        {
            var siteName = "RollbackStartSite";
            using (var s = CreateSite(siteName))
            {
                s.EnsureStop();
                Assert.AreEqual(ObjectState.Stopped, s.State());
                var step = new IisStateStep(new IisApplicationSettings("stop site", siteName, IisApplicationSettings.State.Start));
                step.Apply();
                Assert.AreEqual(ObjectState.Started, s.State());
                step.Rollback();
                Assert.AreEqual(ObjectState.Stopped, s.State());
            }
        }

        [TestMethod]
        public void CreateStarterTest()
        {
            var res = IisStateStep.CreateStarter("start", "site");
            Assert.IsNotNull(res as IisStateStep);
            Assert.AreEqual("start", res.Name);
        }

        [TestMethod]
        public void CreateStoperTest()
        {
            var res = IisStateStep.CreateStoper("start", "site");
            Assert.IsNotNull(res as IisStateStep);
            Assert.AreEqual("start", res.Name);
        }

        [NotNull]
        private TempSite CreateSite([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var directoryInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            directoryInfo.Create();
            using (var sm = new ServerManager())
            {
                var pool = sm.ApplicationPools.Add(name);
                pool.AutoStart = true;

                var site = sm.Sites.Add(name, "http", $"*:80:{name}.local", directoryInfo.FullName);
                site.Applications["/"].ApplicationPoolName = pool.Name;
                sm.CommitChanges();
            }
            //TODO Не успевает примениться и site.State далее падает с COM-exception.
            // 15 установлено методом подбора. Перестает падать где-то на 7, 15 - на всякий случай
            Thread.Sleep(15);
            return new TempSite(directoryInfo.FullName, name);
        }

        private class TempSite : IDisposable
        {
            [NotNull] private readonly string _path;
            [NotNull] private readonly string _site;

            public TempSite([NotNull] string path, [NotNull] string site)
            {
                if (site == null) throw new ArgumentNullException(nameof(site));
                if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
                _path = path;
                _site = site;
            }

            /// <summary>
            ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
            ///     ресурсов.
            /// </summary>
            public void Dispose()
            {
                lock (LockObj)
                {
                    using (var serverManager = new ServerManager())
                    {
                        var site = serverManager.Sites[_site];
                        if (site != null)
                        {
                            serverManager.Sites.Remove(site);
                        }

                        var pool = serverManager.ApplicationPools[_site];
                        if (pool != null)
                        {
                            serverManager.ApplicationPools.Remove(pool);
                        }

                        serverManager.CommitChanges();
                    }
                }
                Directory.Delete(_path, true);
            }

            public void EnsureStart()
            {
                using (var sm = new ServerManager())
                {
                    var site = sm.Sites[_site];
                    if (site.State != ObjectState.Started)
                    {
                        site.Start();
                    }
                    if (site.State != ObjectState.Started)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public void EnsureStop()
            {
                using (var sm = new ServerManager())
                {
                    var site = sm.Sites[_site];
                    if (site.State != ObjectState.Stopped)
                    {
                        site.Stop();
                    }
                    if (site.State != ObjectState.Stopped)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public ObjectState State()
            {
                using (var sm = new ServerManager())
                {
                    var site = sm.Sites[_site];
                    return site.State;
                }
            }
        }
    }
}