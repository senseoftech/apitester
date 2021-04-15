using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sot.ApiTester
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestFileTestDataAttribute : Attribute, ITestDataSource
    {
        private readonly string _folder;
        private const string _extension = "*.json";

        public RequestFileTestDataAttribute(string folderLocation = "Scenarios")
        {
            _folder = folderLocation;
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            var files = Directory.EnumerateFiles(_folder, _extension);
            foreach (var file in files)
            {
                var contentFile = File.ReadAllText(file);
                var payload = JsonConvert.DeserializeObject<Payload>(contentFile);
                yield return new Payload[] {
                    payload
                };
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, Object[] payloadData)
        {
            if (payloadData != null)
            {
                var payload = payloadData[0] as Payload;
                return $"{payload.TestName} (Category: {payload.Category})";
            }
            return null;
        }
    }
}
