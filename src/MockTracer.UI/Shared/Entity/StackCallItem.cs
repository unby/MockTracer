namespace MockTracer.UI.Shared.Entity;

/// <summary>
/// Stack call item
/// </summary>
public class StackCallItem
{
  /// <summary>
  /// class namespace
  /// </summary>
  public string? DeclaringTypeNamespace { get; set; }

  /// <summary>
  /// class name
  /// </summary>
  public string DeclaringTypeName { get; set; }

  /// <summary>
  /// called method name
  /// </summary>
  public string MethodName { get; set; }

  /// <summary>
  /// OutputTypeNamespace
  /// </summary>
  public string? OutputTypeNamespace { get; set; }

  /// <summary>
  /// OutputTypeName
  /// </summary>
  public string OutputTypeName { get; set; }

  /// <summary>
  /// File name
  /// </summary>
  public string? FileName { get; set; }

  /// <summary>
  /// Line number in file
  /// </summary>
  public int? Line { get; set; }
}
