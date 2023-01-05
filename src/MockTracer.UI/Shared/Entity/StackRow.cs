namespace MockTracer.UI.Shared.Entity;

public class StackRow
{
  public Guid Id { get; set; }

  public bool IsEntry { get; set; }

  public DateTime Time { get; set; } = DateTime.Now;

  public int DeepLevel { get; set; } = 1;

  public string StackTrace { get; set; }

  public Guid? ParentId { get; set; }

  public string Title { get; set; }

  public string TracerType { get; set; }

  public List<Input>? Input { get; set; }

  public Output? Output { get; set; }

  public ExceptionType? Exception { get; set; }

  public StackScope Scope { get; set; }

  public Guid ScopeId { get; set; }

  public int Order { get; set; }
}
