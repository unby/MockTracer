namespace MockTracer.UI.Shared.Entity;

/// <summary>
/// Traced object info
/// </summary>
public abstract class TracedObject
{
  /// <summary>
  /// ID
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Argument name
  /// </summary>
  public string Name { get; set; }

  /// <summary>
  /// code name space
  /// </summary>
  public string? Namespace { get; set; }

  /// <summary>
  /// Class name
  /// </summary>
  public string ClassName { get; set; }

  /// <summary>
  /// Object saved as json string
  /// </summary>
  public string Json { get; set; }

  /// <summary>
  /// Object saved as C# code
  /// </summary>
  public string SharpCode { get; set; }

  /// <summary>
  /// 
  /// </summary>
  public string ShortView { get; set; }

  /// <summary>
  /// Stack row ID
  /// </summary>
  public Guid StackRowId { get; set; }

  /// <summary>
  /// Additional info obout traced object
  /// </summary>
  public string? AddInfo { get; set; }

  /// <summary>
  /// Had error during serialze data
  /// </summary>
  public bool IsFilled { get; set; } = true;

  /// <summary>
  /// Full class name
  /// </summary>
  public string FullName => $"{Namespace}.{ClassName}";
}
