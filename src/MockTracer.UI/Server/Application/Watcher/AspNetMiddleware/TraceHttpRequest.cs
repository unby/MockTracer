namespace MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;

public sealed class TraceHttpRequest
{
  public string Path { get; init; }

  public string FullPath { get; init; }

  public string? ContentType { get; init; }

  public string Method { get; init; }
}
