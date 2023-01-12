namespace MockTracer.UI.Server.Application.Watcher;

/// <summary>
/// Defenition input or output argument object
/// </summary>
public class ArgumentObjectInfo
{
  /// <summary>
  /// argument name
  /// </summary>
  public string ArgumentName { get; init; }

  /// <summary>
  /// Original object
  /// </summary>
  public object? OriginalObject { get; init; }

  /// <summary>
  /// additional info about original object
  /// </summary>
  public object? AdvancedInfo { get; init; }

  /// <summary>
  /// namespace
  /// </summary>
  public string? Namespace { get; init; }

  /// <summary>
  /// class name
  /// </summary>
  public string ClassName { get; init; }
}

