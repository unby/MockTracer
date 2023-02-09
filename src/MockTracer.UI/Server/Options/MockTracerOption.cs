using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Options;

/// <summary>
/// tool's options
/// </summary>
public class MockTracerOption
{
  /// <summary>
  /// <see cref="HttpRouteOptions"/>
  /// </summary>
  public HttpRouteOptions AllowRoutes { get; set; } = new HttpRouteOptions();

  /// <summary>
  /// <see cref="ClassGenerationSetting"/>
  /// </summary>
  public ClassGenerationSetting GenerationSetting { get; set; } = new ClassGenerationSetting();
}
