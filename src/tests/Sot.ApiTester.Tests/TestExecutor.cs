using Sot.ApiTester;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sot.ApiTester.Tests
{
    [TestClass]
    public class TestExecutor
    {
        [TestMethod]
        [RequestFileTestData]
        public async Task TestScenarioAsync(Payload payload)
        {
            await Scenario.AssertAsync(payload);
        }
    }
}
