using NUnit.Framework;
using Rib.Deployer.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rib.Deployer.Steps
{
    [TestFixture]
    public class ActionDeploySettingsTests
    {
        [Test]
        public void ActionDeploySettingsNull1Test()  => Assert.Throws<ArgumentNullException>(() =>  new ActionDeploySettings("name", null, () => {}));

        [Test]
        public void ActionDeploySettingsNull2Test()  => Assert.Throws<ArgumentNullException>(() =>  new ActionDeploySettings("name", () => { }, null));

        [Test]
        public void ActionDeploySettingsNull3Test()  => Assert.Throws<ArgumentException>(() =>  new ActionDeploySettings(null, () => { }, () => {}));

        [Test]
        public void ActionDeploySettingsEmptyTest()  => Assert.Throws<ArgumentException>(() =>  new ActionDeploySettings(string.Empty, () => { }, () => { }));

        [Test]
        public void ActionDeploySettingsWhiteSpaceTest()  => Assert.Throws<ArgumentException>(() =>  new ActionDeploySettings("      ", () => { }, () => { }));
    }
}