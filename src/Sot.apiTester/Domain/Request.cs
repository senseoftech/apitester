using Sot.ApiTester.Extensions;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Sot.ApiTester.Domain
{
    public class Request
    {
        public string Path { get; set; }
        public string BaseUrl { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public JObject Content { get; set; }
        public JObject Header { get; set; }

        public HttpRequestMessage RetrieveHttpRequestMessage(Payload payload)
        {
            var request = new HttpRequestMessage(Method.ToHttpMethod(), Path);

            SetRequestContent(request, payload);
            SetRequestHeader(request, payload);

            return request;
        }

        private void SetRequestContent(HttpRequestMessage request, Payload payload)
        {
            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
            {
                string contentType;
                string requestContent;
                if (string.Compare("form", ContentType, true) == 0)
                {
                    contentType = "application/x-www-form-urlencoded";
                    requestContent = GetRequestBodyRawContentWithReplacement(payload);
                }
                else
                {
                    contentType = "application/json";
                    requestContent = GetRequestContentWithReplacement(payload);
                }
                request.Content = new StringContent(requestContent, Encoding.UTF8, contentType);
            }
        }

        private void SetRequestHeader(HttpRequestMessage request, Payload payload)
        {
            if (Header != null)
            {
                var headers = JObject.Parse(Header.ToString().SubstituteTokens(payload));
                foreach (var item in headers.Children<JToken>().Select(e => (JProperty)e))
                {
                    request.Headers.TryAddWithoutValidation(item.Name, item.Value.ToString());
                }
            }
        }

        private string GetRequestBodyRawContentWithReplacement(Payload payload)
        {
            var query = string
                .Join('&', Content
                    .Children<JToken>()
                    .Select(e => (JProperty)e)
                    .Select(e => $"{e.Name}={e.Value}"));

            return query.SubstituteTokens(payload);
        }

        private string GetRequestContentWithReplacement(Payload payload)
        {
            return Content.ToString().SubstituteTokens(payload);
        }
    }
}
