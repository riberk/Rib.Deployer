using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rib.Deployer.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rib.Deployer.Steps
{
    [TestClass]
    public class ActionDeploySettingsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActionDeploySettingsNull1Test() => new ActionDeploySettings("name", null, () => {});

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActionDeploySettingsNull2Test() => new ActionDeploySettings("name", () => { }, null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsNull3Test() => new ActionDeploySettings(null, () => { }, () => {});

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsEmptyTest() => new ActionDeploySettings(string.Empty, () => { }, () => { });

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActionDeploySettingsWhiteSpaceTest() => new ActionDeploySettings("      ", () => { }, () => { });
    }
}