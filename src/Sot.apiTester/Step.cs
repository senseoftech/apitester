using Sot.ApiTester.Domain;
using Sot.ApiTester.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sot.ApiTester
{
    public static class Step
    {
        public static async Task AssertAsync(Sot.ApiTester.Domain.Step step, Payload payload, string defaultUrl)
        {
            // Arrange
            var baseAddress = new Uri(string.IsNullOrEmpty(step.Request.BaseUrl)
                ? defaultUrl
                : step.Request.BaseUrl);

            HttpClient httpClient = new()
            {
                BaseAddress = baseAddress
            };

            var request = step.Request.RetrieveHttpRequestMessage(payload);

            // Act
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, step.Response.GetHttpStatusCode());
            JToken jObject = JToken.Parse(await response.Content.ReadAsStringAsync());
            step.Response.ReturnedResponse = jObject;

            if (step.Response.ExpectedContentResult == null)
            {
                Assert.IsFalse(jObject.HasValues, "The result message contains response and the expected result should be empty.");
            }
            else
            {
                var expectedNode = step.Response.ExpectedContentResult;
                NodeValidators.AssertEquals(expectedNode, jObject);
            }
        }
    }
}
