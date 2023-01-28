namespace MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;

/// <summary>
/// Http request defenition
/// </summary>
public sealed class TraceHttpRequest
{
  public const string DefaultPath = "http://path/";

  /// <summary>
  /// TraceHttpRequest
  /// </summary>
  /// <param name="path">http path segment</param>
  /// <param name="fullPath">http path</param>
  /// <param name="contentType">content type</param>
  /// <param name="method">http method</param>
  public TraceHttpRequest(string? path, string? fullPath, string? contentType, string method)
  {
    Path = path ?? string.Empty;
    FullPath = fullPath ?? DefaultPath;
    ContentType = contentType;
    Method = method;
  }

  /// <summary>
  /// http path segment
  /// </summary>
  public string Path { get; init; }
  /// <summary>
  /// http path
  /// </summary>
  public string FullPath { get; init; }

  /// <summary>
  /// content type
  /// </summary>
  public string? ContentType { get; init; }

  /// <summary>
  /// http method
  /// </summary>
  public string Method { get; init; }
}
