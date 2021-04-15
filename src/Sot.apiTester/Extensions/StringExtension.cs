using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Sot.ApiTester.Extensions
{
    public static class StringExtension
    {
        public static HttpMethod ToHttpMethod(this string method)
           => (method.ToUpper()) switch
           {
               "GET" => HttpMethod.Get,
               "POST" => HttpMethod.Post,
               "PUT" => HttpMethod.Put,
               "DELETE" => HttpMethod.Delete,
               "TRACE" => HttpMethod.Trace,
               "HEAD" => HttpMethod.Head,
               "OPTIONS" => HttpMethod.Options,
               "PATCH" => HttpMethod.Patch,
               _ => throw new System.ArgumentOutOfRangeException("Method is not available or wrong"),
           };

        public static string SubstituteTokens(this string requestContent, Payload payload)
        {
            var patternRegex = new Regex("((#([a-zA-Z0-9_\\-]+)\\/([^#]+)#))");
            if (patternRegex.IsMatch(requestContent))
            {
                var matches = patternRegex.Matches(requestContent);
                foreach (Match match in matches)
                {
                    var tokens = match.Groups[1].Value;
                    var id = match.Groups[3].Value;
                    var path = match.Groups[4].Value;

                    var elementForReplace = payload.Steps.FirstOrDefault(s => string.Compare(s.Id, id, true) == 0);

                    if (elementForReplace == null)
                        throw new ArgumentException($"The request id : {id} is not found.");

                    var valueToReplace = elementForReplace.Response.ReturnedResponse.SelectToken(path);

                    if (valueToReplace == null)
                        throw new ArgumentException($"The path {path} in the request with id : {id} is not found.");

                    requestContent = requestContent.Replace(tokens, valueToReplace.Value<string>());
                }
            }

            return requestContent;
        }
    }
}
