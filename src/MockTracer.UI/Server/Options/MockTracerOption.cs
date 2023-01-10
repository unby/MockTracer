namespace MockTracer.UI.Server.Options;

public class MockTracerOption
{
  public HttpRouteOptions AllowRoutes { get; set; } = new HttpRouteOptions();

  public ClassGenerationSetting GenerationSetting { get; set; } = new ClassGenerationSetting();
}
