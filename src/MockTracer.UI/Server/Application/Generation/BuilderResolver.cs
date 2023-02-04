using Microsoft.Extensions.DependencyInjection;
using MockTracer.UI.Server.Application.Generation.MockBuilders;
using MockTracer.UI.Server.Application.Generation.TraceBuilders;

namespace MockTracer.UI.Server.Application.Generation;

/// <summary>
/// XUnit code builder
/// </summary>
public class BuilderResolver : IBuilderResolver
{
  private readonly IServiceProvider _service;

  /// <summary>
  /// BuilderResolver
  /// </summary>
  /// <param name="service"><see cref="IServiceProvider"/></param>
  public BuilderResolver(IServiceProvider service)
  {
    _service = service;
  }

  /// <inheritdoc/>
  public InputPointBuilderBase ResolveInputBuilder(string tracerCode)
  {
    switch (tracerCode)
    {
      case Constants.HttpContext:
        return _service.GetRequiredService<HttpContextBuilder>();
      case Constants.Mediatr:
        return _service.GetRequiredService<MediatrInputBuilder>();
      case Constants.MvcActionFilter:
        return _service.GetRequiredService<MvcFilterBuilder>();
      case Constants.Custom:
        return _service.GetRequiredService<CustomInputBuilder>();
      default:
        throw new NotImplementedException(tracerCode);
    }
  }

  /// <inheritdoc/>
  public MockPointBuilderBase ResolveMockBuilder(string tracerCode)
  {
    switch (tracerCode)
    {
      case Constants.HttpContext:
      case Constants.MvcActionFilter:
        return _service.GetRequiredService<FakeMockBuilder>();
      case Constants.Mediatr:
        return _service.GetRequiredService<MediatorMockBuilder>();
      case Constants.HttpClient:
        return _service.GetRequiredService<HttpClientMockBuilder>();
      case Constants.DbConnection:
        return _service.GetRequiredService<DbConnectionMockBuilder>();
      case Constants.Custom:
        return _service.GetRequiredService<CustomMockBuilder>();
      default:
        throw new NotImplementedException(tracerCode);
    }
  }

  /// <inheritdoc/>
  public ITemplateBuilder ResolveTemplateBuilder(string templateCode)
  {
    return _service.GetRequiredService<XunitMockTemplateBuilder>();
  }
}
