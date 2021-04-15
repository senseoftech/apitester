using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Sot.ApiTester.Validators
{
    internal static class NodeValidators
    {
        public static void AssertEquals(JToken expectedNode, JToken jObject)
        {
            if (jObject is null)
            {
                Assert.IsNull(expectedNode);
            }
            else
            {
                AssertJArray(expectedNode, jObject);
                AssertJObject(expectedNode, jObject);
            }
        }

        private static void AssertJArray(JToken expectedNode, JToken responseNode)
        {
            if (expectedNode is JArray)
            {
                Assert.IsTrue(responseNode is JArray, "The node is not an array");

                foreach (var childNode in expectedNode)
                {
                    if (childNode is JValue)
                    {
                        if (expectedNode.HasValues && HasNotOptional(expectedNode) && !responseNode.Any())
                        {
                            Assert.Fail($"The node {responseNode.Path} should not be empty.");
                        }
                        foreach (var childResponseNode in responseNode)
                        {
                            AssertJValue(childNode, childResponseNode);
                        }
                    }
                    foreach (var childResponseNode in responseNode)
                    {
                        switch (childResponseNode)
                        {
                            case JObject jObject:
                                AssertEquals(childNode, jObject);
                                break;
                            case JArray array:
                                AssertJArray(childNode, array);
                                break;
                        }
                    }
                }
            }
        }

        private static bool HasNotOptional(JToken expectedNode)
        {
            foreach (var child in expectedNode)
            {
                var type = child.Value<string>();
                if (!type.Contains('?'))
                    return true;
            }
            return false;
        }

        private static void AssertJObject(JToken expectedNode, JToken response)
        {
            if (expectedNode is JObject)
            {
                Assert.IsTrue(response is JObject, "The node is not an object");
                JObject node = expectedNode as JObject;
                JObject responseNode = response as JObject;
                foreach (JProperty item in node.Children())
                {
                    var name = item.Name;
                    var expectedTypeValue = item.Value;
                    JProperty responseProperty = GetNodeFromResponseNode(responseNode, name);
                    if (!expectedTypeValue.Value<string>().Contains("?") && responseProperty == null)
                    {
                        Assert.IsNotNull(responseProperty, $"The property {name} is not found on the item {response.Path}.");
                    }
                    else if (!expectedTypeValue.Value<string>().Contains("?") && responseProperty.Value != null)
                    {
                        Assert.IsNotNull(responseProperty.Value, $"The node {responseProperty.Path} can't be empty");
                        switch (responseProperty.Value)
                        {
                            case JValue value:
                                AssertJValue(expectedTypeValue, value);
                                break;
                            case JArray array:
                                AssertJArray(expectedTypeValue, array);
                                break;
                            case JObject valueObject:
                                AssertJObject(expectedTypeValue, valueObject);
                                break;
                        }
                    }
                }
            }
        }

        private static void AssertJValue(JToken expectedJValue, JToken responseJValue)
        {
            if (expectedJValue is JValue)
            {
                Assert.IsTrue(responseJValue is JValue, $"The node {responseJValue.Parent.Path} should be a value. The node is {responseJValue.GetType()}.");
                var expectedType = "";
                try
                {
                    switch (expectedJValue.Value<string>())
                    {
                        case "string":
                        case "string?":
                            expectedType = "string";
                            var value = responseJValue.Value<string>();
                            break;
                        case "datetime":
                        case "datetime?":
                        case "date":
                        case "date?":
                            expectedType = "datetime";
                            var valueDateTime = responseJValue.Value<DateTime>();
                            break;
                        case "guid":
                        case "guid?":
                            expectedType = "guid";
                            var valueGuid = responseJValue.Value<Guid>();
                            break;
                        case "number":
                        case "number?":
                            expectedType = "number";
                            var valueNumber = responseJValue.Value<long>();
                            break;
                        case "decimal":
                        case "decimal?":
                            expectedType = "decimal";
                            var valueDecimalNumber = responseJValue.Value<decimal>();
                            break;
                        case "boolean":
                        case "boolean?":
                            expectedType = "boolean";
                            var valueBoolean = responseJValue.Value<bool>();
                            break;
                    }
                }
                catch (Exception)
                {
                    Assert.Fail($"The node {responseJValue.Parent.Path} should be a {expectedType}. Value received: {responseJValue}");
                }
            }
        }

        private static JProperty GetNodeFromResponseNode(JObject responseNode, string name)
        {
            return responseNode.Children()
                .Select(d => (JProperty)d)
                .FirstOrDefault(c => string.Compare(c.Name, name, true) == 0);
        }
    }
}
