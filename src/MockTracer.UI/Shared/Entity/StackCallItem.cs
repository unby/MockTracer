namespace MockTracer.UI.Shared.Entity;

public class StackCallItem
{
  public string? DeclaringTypeNamespace { get; set; }
  public string DeclaringTypeName { get; set; }
  public string MethodName { get; set; }
  public string? OutputTypeNamespace { get; set; }
  public string OutputTypeName { get; set; }
  public string? FileName { get; set; }
  public int? Line { get; set; }
}
