using System.Collections.Generic;

namespace Sot.ApiTester
{
    public class Payload
    {
        public Payload()
        {
            Steps = new List<Domain.Step>();
        }

        public string TestName { get; set; }
        public string Category { get; set; }
        public IEnumerable<Domain.Step> Steps { get; set; }
    }
}
