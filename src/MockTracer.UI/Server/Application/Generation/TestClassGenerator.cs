using Microsoft.Extensions.Options;
using MockTracer.UI.Server.Application.Presentation;
using MockTracer.UI.Server.Options;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Application.Generation;

public class TestClassGenerator
{
  private readonly TraceRepository _traceRepository;
  private readonly IBuilderResolver _builderResolver;
  private ClassGenerationSetting _options;

  public TestClassGenerator(IOptions<ClassGenerationSetting> options, TraceRepository traceRepository, IBuilderResolver builderResolver)
  {
    _options = options.Value;
    _traceRepository = traceRepository;
    _builderResolver = builderResolver;
  }

  public async Task<TestFile> CreateAsync(GenerationAttributes @params)
  {
    var data = await _traceRepository.GetGenerationDataAsync(@params);
    var builder = _builderResolver.ResolveTemplateBuilder(@params.TemplateCode);

    return new TestFile()
    {
      FileName = "test.cs",
      SourceCode = builder.Build(
        @params,
       _options,
       data)
    };
  }
}
