namespace MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;

/// <summary>
/// HttpReponse info
/// </summary>
public sealed class TraceHttpReponse
{
  /// <summary>
  /// StatusCode
  /// </summary>
  public int StatusCode { get; init; }

  /// <summary>
  /// ContentType
  /// </summary>
  public string? ContentType { get; init; }
}
