namespace MockTracer.UI.Server.Application.Generation;

public class XunitMockTemplateBuilder : TemplateBuilderBase
{
  public XunitMockTemplateBuilder(IBuilderResolver builderResolver) : base(builderResolver)
  {
  }

  protected override string Template => @"{{using}}
using Xunit.Abstractions;

namespace {{nameSpace}}{{nameSpace[}}

public class {{className}} : {{TestBase}}
{
  public {{className}}(ITestOutputHelper output)
      : base(output)
  {  }

  [Fact]
  public async Task {{methodName}}Async()
  {
    // Arrange
    {{Prepare}}

    // Act
    var host = NewServer((s) => {{Configure}});
    {{Action}}

    // todo: Assert
    {{Assert}}
  }{{BigVariable}}
}{{nameSpace]}}";
}
