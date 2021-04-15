using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json.Serialization;

namespace Sot.ApiTester.Domain
{
    public class Response
    {
        public int ResultCode { get; set; }
        public JToken ExpectedContentResult { get; set; }

        public HttpStatusCode GetHttpStatusCode() => (HttpStatusCode)ResultCode;

        [JsonIgnore]
        public JToken ReturnedResponse { get; set; }
    }
}