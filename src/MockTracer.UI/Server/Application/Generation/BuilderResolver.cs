using Microsoft.Extensions.Logging;
using MockTracer.UI.Server.Application.Generation.MockBuilders;
using MockTracer.UI.Server.Application.Generation.TraceBuilders;

namespace MockTracer.UI.Server.Application.Generation;

public class BuilderResolver : IBuilderResolver
{
  private readonly IServiceProvider _service;

  public BuilderResolver(IServiceProvider service)
  {
    _service = service;
  }
  public TracerBuilderBase ResolveInputBuilder(string tracerCode)
  {
    switch (tracerCode)
    {
      case Constants.HttpContext:
        return _service.GetRequiredService<HttpContextBuilder>();
      case Constants.Mediatr:
        throw new NotImplementedException(tracerCode);
      case Constants.MvcActionFilter:
        return _service.GetRequiredService<MvcFilterBuilder>();
      case Constants.DbConnection:
      case Constants.Custom:
      default:
        throw new NotImplementedException(tracerCode);
    }
  }

  public MockBuilderBase ResolveMockBuilder(string tracerCode)
  {
    switch (tracerCode)
    {
      case Constants.HttpContext:
        return _service.GetRequiredService<HttpContextMockBuilder>();
      case Constants.Mediatr:
        return _service.GetRequiredService<MediatorMockBuilder>();
      case Constants.HttpClient:
        return _service.GetRequiredService<HttpClientMockBuilder>();
      case Constants.DbConnection:
        return _service.GetRequiredService<DbConnectionMockBuilder>();
      case Constants.Custom:
      case Constants.MvcActionFilter:
      default:
        throw new NotImplementedException(tracerCode);
    }
  }

  public ITemplateBuilder ResolveTemplateBuilder(string templateCode)
  {
    return _service.GetRequiredService<XunitMockTemplateBuilder>();
  }
}
