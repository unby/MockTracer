namespace MockTracer.UI.Shared.Entity;

public abstract class TracedObject
{
  public Guid Id { get; set; }

  public string Name { get; set; }

  public string Namespace { get; set; }

  public string ClassName { get; set; }

  public string Json { get; set; }

  public string SharpCode { get; set; }

  public string ShortView { get; set; }

  public Guid StackRowId { get; set; }

  public string? AddInfo { get; set; }

  public string FullName => $"{Namespace}.{ClassName}";
}
