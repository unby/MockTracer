using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MockTracer.UI.Server.Options;

public class MockTracerOption
{
  public string SkipDefaultNamespace { get; set; } = "MockTracer.UI";

  public string SkipPath { get; set; } = "/MockTracer";

  public HttpRouteOptions AllowRoutes { get; set; } = new HttpRouteOptions();

  public ClassGenerationSetting GenerationSetting { get; set; } = new ClassGenerationSetting();

  public Action<MockTracerOption, IServiceCollection, IConfiguration> TraceReplacer { get; set; } = (o, s, c) => { };
}
