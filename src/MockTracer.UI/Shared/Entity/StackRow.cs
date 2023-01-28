namespace MockTracer.UI.Shared.Entity;

/// <summary>
/// Stack point info
/// </summary>
public class StackRow
{
  /// <summary>
  /// ID
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// is first entry
  /// </summary>
  public bool IsEntry { get; set; }

  /// <summary>
  /// time call
  /// </summary>
  public DateTime Time { get; set; } = DateTime.Now;

  /// <summary>
  /// stack deep
  /// </summary>
  public int DeepLevel { get; set; } = 1;

  /// <summary>
  /// stack trace
  /// </summary>
  public string StackTrace { get; set; }

  /// <summary>
  /// Parent stack id
  /// </summary>
  public Guid? ParentId { get; set; }

  /// <summary>
  /// title row
  /// </summary>
  public string Title { get; set; }

  /// <summary>
  /// tracer type
  /// </summary>
  public string TracerType { get; set; }

  /// <summary>
  /// input args
  /// </summary>
  public List<Input>? Input { get; set; }

  /// <summary>
  /// output arg
  /// </summary>
  public Output? Output { get; set; }

  /// <summary>
  /// cought exception
  /// </summary>
  public ExceptionType? Exception { get; set; }

  /// <summary>
  /// Scope
  /// </summary>
  public StackScope Scope { get; set; }

  /// <summary>
  /// ScopeId
  /// </summary>
  public Guid ScopeId { get; set; }

  /// <summary>
  /// Call order
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// class namespace
  /// </summary>
  public string? DeclaringTypeNamespace { get; set; }

  /// <summary>
  /// class name
  /// </summary>
  public string? DeclaringTypeName { get; set; }

  /// <summary>
  /// called method name
  /// </summary>
  public string? MethodName { get; set; }

  /// <summary>
  /// OutputTypeNamespace
  /// </summary>
  public string? OutputTypeNamespace { get; set; }

  /// <summary>
  /// OutputTypeName
  /// </summary>
  public string? OutputTypeName { get; set; }
}
