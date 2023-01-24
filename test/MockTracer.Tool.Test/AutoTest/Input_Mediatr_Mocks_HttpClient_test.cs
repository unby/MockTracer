using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.Tool.Test;

public class Mediatr : ToolTestBase
{
  public Mediatr(Xunit.Abstractions.ITestOutputHelper output)
    : base(output)
  {
  }

  [Fact]
  public async Task Should_create_HttpContext_template_Async()
  {
    var generator = NewServer().Services.GetService<TestClassGenerator>();
    var newClass = await generator.CreateAsync(Attributes);

    Assert.NotNull(newClass.SourceCode);
  }

  protected GenerationAttributes Attributes => new GenerationAttributes
  {
    ScopeId = new Guid("08dafe2c-9771-1fdf-c87e-ba4824021c37"),
    InputId = new Guid("08dafe2c-9788-85b4-c87e-ba4824021c3c"),
    OutputId = new Guid[]
  {
    new Guid("08dafe2c-9797-c926-c87e-ba4824021c3e")
  },
    TestName = "testclass.testmethod"
  };

  protected override void AfterBuildConfiguration(IHost host)
  {
    base.AfterBuildConfiguration(host);
    using var scope = host.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<MockTracerDbContext>();
    context.AddRange(new StackScope
    {
      Id = new Guid("08dafe2c-9771-1fdf-c87e-ba4824021c37"),
      Time = DateTime.ParseExact("2023-01-24T22:01:09.9400884", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
      Title = "/api/topic/v10/fact",
      FirstType = "HttpContext",
      FirstId = new Guid("08dafe2c-9775-0475-c87e-ba4824021c38"),
      Stack = new List<StackRow>
  {
    new StackRow
    {
      Id = new Guid("08dafe2c-9788-85b4-c87e-ba4824021c3c"),
      Time = DateTime.ParseExact("2023-01-24T22:01:10.0669407", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
      DeepLevel = 3,
      StackTrace = "[\r\n  {\r\n    \"DeclaringTypeNamespace\": \"MockTracer.Test.Api.Controllers\",\r\n    \"DeclaringTypeName\": \"\\u003CGetfactAsync\\u003Ed__4\",\r\n    \"MethodName\": \"MoveNext\",\r\n    \"OutputTypeNamespace\": \"System\",\r\n    \"OutputTypeName\": \"Void\",\r\n    \"FileName\": \"C:\\\\git\\\\MoqTracer\\\\test\\\\MockTracer.Test.Api\\\\Controllers\\\\TopicController.cs\",\r\n    \"Line\": 56\r\n  }\r\n]",
      ParentId = new Guid("08dafe2c-9785-afa1-c87e-ba4824021c3a"),
      Title = "MockTracer.Test.Api.Application.Features.Topic.CatFactQuery",
      TracerType = "Mediatr",
      Input = new List<Input>
      {
        new Input
        {
          Id = new Guid("08dafe2c-9788-bd2d-c87e-ba4824021c3d"),
          Name = "request",
          Namespace = "MockTracer.Test.Api.Application.Features.Topic",
          ClassName = "CatFactQuery",
          Json = "{\r\n  \"FactId\": null\r\n}",
          SharpCode = "new CatFactQuery\r\n{\r\n}",
          ShortView = "",
          StackRowId = new Guid("08dafe2c-9788-85b4-c87e-ba4824021c3c"),
          IsFilled = true
        }
      },
      Output = new Output
      {
        Id = new Guid("08dafe2c-9d3b-809e-c87e-ba4824021c41"),
        Name = "response",
        Namespace = "MockTracer.Test.Api.Domain",
        ClassName = "CatFact",
        Json = "{\r\n  \"Fact\": \"Cats take between 20-40 breaths per minute.\",\r\n  \"Length\": 43\r\n}",
        SharpCode = "new CatFact\r\n{\r\n  Fact = \"Cats take between 20-40 breaths per minute.\",\r\n  Length = 43\r\n}",
        ShortView = "",
        StackRowId = new Guid("08dafe2c-9788-85b4-c87e-ba4824021c3c"),
        IsFilled = true
      },
      Scope = null, // Circular reference detected
      ScopeId = new Guid("08dafe2c-9771-1fdf-c87e-ba4824021c37"),
      Order = 3
    },
    new StackRow
    {
      Id = new Guid("08dafe2c-9797-c926-c87e-ba4824021c3e"),
      Time = DateTime.ParseExact("2023-01-24T22:01:10.1669974", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
      DeepLevel = 4,
      StackTrace = "[\r\n  {\r\n    \"DeclaringTypeNamespace\": \"Refit\",\r\n    \"DeclaringTypeName\": \"Refit.RequestBuilderImplementation\\u002B\\u003C\\u003Ec__DisplayClass14_0\\u00602\\u002B\\u003C\\u003CBuildCancellableTaskFuncForMethod\\u003Eb__0\\u003Ed**FAILEd_PARSE**\",\r\n    \"MethodName\": \"MoveNext\",\r\n    \"OutputTypeNamespace\": \"System\",\r\n    \"OutputTypeName\": \"Void\",\r\n    \"FileName\": \"/_/Refit/RequestBuilderImplementation.cs\",\r\n    \"Line\": 256\r\n  },\r\n  {\r\n    \"DeclaringTypeNamespace\": \"Refit\",\r\n    \"DeclaringTypeName\": \"\\u003C\\u003Ec__DisplayClass22_0\\u003CRefit.T,Refit.TBody\\u003E\",\r\n    \"MethodName\": \"\\u003CBuildTaskFuncForMethod\\u003Eb__0\",\r\n    \"OutputTypeNamespace\": \"System.Threading.Tasks\",\r\n    \"OutputTypeName\": \"Task\\u003CRefit.T\\u003E\",\r\n    \"FileName\": \"/_/Refit/RequestBuilderImplementation.cs\",\r\n    \"Line\": 856\r\n  },\r\n  {\r\n    \"DeclaringTypeNamespace\": \"Refit.Implementation\",\r\n    \"DeclaringTypeName\": \"MockTracerTestApiInfrastractureExternalICatService\",\r\n    \"MethodName\": \"global::MockTracer.Test.Api.Infrastracture.External.ICatService.GetCatFactAsync\",\r\n    \"OutputTypeNamespace\": \"System.Threading.Tasks\",\r\n    \"OutputTypeName\": \"Task\\u003CMockTracer.Test.Api.Domain.CatFact\\u003E\",\r\n    \"FileName\": \"C:\\\\git\\\\MoqTracer\\\\test\\\\MockTracer.Test.Api\\\\InterfaceStubGeneratorV2\\\\Refit.Generator.InterfaceStubGeneratorV2\\\\ICatService.g.cs\",\r\n    \"Line\": 45\r\n  },\r\n  {\r\n    \"DeclaringTypeNamespace\": \"MockTracer.Test.Api.Application.Features.Topic\",\r\n    \"DeclaringTypeName\": \"\\u003CHandle\\u003Ed__2\",\r\n    \"MethodName\": \"MoveNext\",\r\n    \"OutputTypeNamespace\": \"System\",\r\n    \"OutputTypeName\": \"Void\",\r\n    \"FileName\": \"C:\\\\git\\\\MoqTracer\\\\test\\\\MockTracer.Test.Api\\\\Application\\\\Features\\\\Cats\\\\CatFactQueryHandler.cs\",\r\n    \"Line\": 20\r\n  },\r\n  {\r\n    \"DeclaringTypeNamespace\": \"MockTracer.Test.Api.Controllers\",\r\n    \"DeclaringTypeName\": \"\\u003CGetfactAsync\\u003Ed__4\",\r\n    \"MethodName\": \"MoveNext\",\r\n    \"OutputTypeNamespace\": \"System\",\r\n    \"OutputTypeName\": \"Void\",\r\n    \"FileName\": \"C:\\\\git\\\\MoqTracer\\\\test\\\\MockTracer.Test.Api\\\\Controllers\\\\TopicController.cs\",\r\n    \"Line\": 56\r\n  }\r\n]",
      ParentId = new Guid("08dafe2c-9788-85b4-c87e-ba4824021c3c"),
      Title = "/fact",
      TracerType = "HttpClient",
      Input = new List<Input>
      {
        new Input
        {
          Id = new Guid("08dafe2c-9797-ce25-c87e-ba4824021c3f"),
          Name = "request",
          Namespace = "System.Net.Http",
          ClassName = "HttpRequestMessage",
          Json = "null",
          SharpCode = "null",
          ShortView = "",
          StackRowId = new Guid("08dafe2c-9797-c926-c87e-ba4824021c3e"),
          AddInfo = "{\r\n  \"Path\": \"/fact\",\r\n  \"FullPath\": \"https://catfact.ninja/fact\",\r\n  \"ContentType\": null,\r\n  \"Method\": \"GET\"\r\n}",
          IsFilled = true
        }
      },
      Output = new Output
      {
        Id = new Guid("08dafe2c-9d38-4965-c87e-ba4824021c40"),
        Name = "response",
        Namespace = "MockTracer.Test.Api.Domain",
        ClassName = "CatFact",
        Json = "{\r\n  \"Fact\": \"Cats take between 20-40 breaths per minute.\",\r\n  \"Length\": 43\r\n}",
        SharpCode = "new CatFact\r\n{\r\n  Fact = \"Cats take between 20-40 breaths per minute.\",\r\n  Length = 43\r\n}",
        ShortView = "",
        StackRowId = new Guid("08dafe2c-9797-c926-c87e-ba4824021c3e"),
        AddInfo = "{\r\n  \"StatusCode\": 200,\r\n  \"ContentType\": \"application/json\"\r\n}",
        IsFilled = true
      },
      Scope = null, // Circular reference detected
      ScopeId = new Guid("08dafe2c-9771-1fdf-c87e-ba4824021c37"),
      Order = 4
    }
  }
    });
    context.SaveChanges();
  }
}
