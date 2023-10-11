using System.Reflection;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Watcher;

/// <summary>
/// Info about point in stack
/// </summary>
public class TraceInfo
{
  /// <summary>
  /// ID
  /// </summary>
  public Guid TraceId { get; init; }

  /// <summary>
  /// StackTrace fragment
  /// </summary>
  public StackCallItem[] StackTrace { get; init; } = FragmentExtention.ReadTrace();

  /// <summary>
  /// TracerType
  /// </summary>
  public string TracerType { get; init; }

  /// <summary>
  /// Title
  /// </summary>
  public string Title { get; init; }

  /// <summary>
  /// Called type
  /// </summary>
  public Type? CalledType { get; init; }

  /// <summary>
  /// Called method
  /// </summary>
  public MethodInfo? CalledMethod { get; init; }

  public Type? OutputType { get; init; }
}

