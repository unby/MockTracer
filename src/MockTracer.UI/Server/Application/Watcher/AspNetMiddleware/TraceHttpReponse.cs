namespace MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;

public sealed class TraceHttpReponse
{
  public int StatusCode { get; init; }

  public string ContentType { get; init; }
}
