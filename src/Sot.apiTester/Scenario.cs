using System.Threading.Tasks;

namespace Sot.ApiTester
{
    public static class Scenario
    {
        public static async Task AssertAsync(Payload payload, string defaultUrl = "https://localhost:5001")
        {
            foreach (var step in payload.Steps)
            {
                await Step.AssertAsync(step, payload, defaultUrl);
            }
        }
    }
}
