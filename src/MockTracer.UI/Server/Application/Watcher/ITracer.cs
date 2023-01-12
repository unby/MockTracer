namespace MockTracer.UI.Server.Application.Watcher;

/// <summary>
/// Data aggregator of stack trace
/// </summary>
public interface ITracer
{
  /// <summary>
  /// Create trace info
  /// </summary>
  /// <param name="title">specific info</param>
  /// <returns><see cref="TraceInfo"/></returns>
  TraceInfo CreateInfo(string title);
}
