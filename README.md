# Api Testers
Current version : 0.3.0

[![Build Status](https://dev.azure.com/senseoftech/GitHub/_apis/build/status/senseoftech.apitester?branchName=main)](https://dev.azure.com/senseoftech/GitHub/_build/latest?definitionId=7&branchName=main)

This is a project based on Microsoft MsTest play to play HTTP's stories. 

## How it works. 

This test application is based on XUnit to run test based on json document. 
The documents are read via an Xunit theory decorator. 

Each document will interpreted and executed one at a time. (Limitation MS Test and XUnit - Optimization to think)

## How to works 

Create a new MSTest project or reuse one with the MSTest dependencies installed. 

On the a test class you need to use a method with the following signature: 

```cs
using Sot.ApiTester;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MyNamespace
{
    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        [RequestFileTestData]
        public async Task TestScenarioAsync(Payload payload)
        {
            await Scenario.AssertAsync(payload);
        }
    }
}
```

## Scenario file

The story file need to be stored in the "stories" folder. 
Each document is considered as a unique contextual story.

### Scenario file location

You need to include your scenarios files into a specific folder called ´´´Scenarios´´´

You need to add the "copyalways" or "copyifnewer" flag to your scenarios files. 

Here is a example to include all files in the flag : 

 ```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <None Update="Scenarios\*.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
 ```

 You can change the folder location by overriding the parameter in ```folderLocation``` in the data attribute ```[RequestFileTestData]``` : 

 ```cs

[TestMethod]
[RequestFileTestData(folderLocation = "OtherFolder")]
public async Task TestScenarioAsync(Payload payload)
{
    await Scenario.AssertAsync(payload);
}


 ```

### Scenario file format 

You need to include json files into the ```Scenarios``` folder.

 ```json 
{
  "testName": "Test-login with businessUnit", // Name of the test story
  "category": "Integration", // Name of the category 
  "steps": [ // Define an array of all request to execute 
    {
      "id": "authentication", // Identification used for variable substitution
      "name": "authentication", // Name of the step 
      "request": { // Define all data needed to make the request 
        "method": "POST", // HTTP Method (GET, POST, PUT, DELETE, ...)
        "path": "token", // Path to ressource 
        "baseUrl": "http://localhost:40351", // Base url of the service
        "contentType": "form", // Content type (by default : json) You can use the term "form" to use application/x-form-application, to simulate a form post.
         "header": { // Header of the request (used essentially for authorization
          "authorization": "#authentication/token_type# #authentication/access_token#" // Authorization header, value see in section variable substitution
        }
        "content": { // Define a representation of the content request 
          
        }
      },
      "response": { // Define the expected response 
        "resultCode": 200, // Expected status result code 
        "expectedContentResult": { // Define the expect result 
          "access_token": "string",
          "token_type": "string",
          "expires_in": "number"
        }
      }
    },...
  ]
}
``` 

#### Supported expected result 

In the section steps[...].response.expectedContentResult, you can define an object, an array or a property in JSON format. 

The representation is a standard json representation. You can validate result by replacing the value by a type. 

Available types : 

| Type      | C# representation |
|-----------|-------------------|
| string    | string            |
| datetime  | datetime          |
| date      | datetime          |
| guid      | System.Guid       |
| number    | long              |
| decimal   | decimal           |
| boolean   | bool              |


All of these types can be nullable by suffixing a ? at the end. Exemple : number? 

#### Variable substitution 

You can substitute variable in header based on result of previous step. 

Each step is identified by an Id (string) and you can use the following syntax to replace the token by a value. 
If the value is not found, the story will be stopped. 

Syntax : 

    #[IDENTIFIER]/[PATH]#

Exemple : 

Base on a first step with the id : authentication and the response contains the property access_token at the result's root. You can replace the header authorization like that :
 ```json 
 "header": { 
    "authorization": "bearer #authentication/access_token#"
 }
```

This feature is only available on request.header and request.content. 

The path part is based on the pluging Newtonsoft  : https://www.newtonsoft.com/json/help/html/SelectToken.htm

## Improvement

- Include step from another json document.
