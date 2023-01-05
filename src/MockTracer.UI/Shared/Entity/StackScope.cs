namespace MockTracer.UI.Shared.Entity;

public class StackScope
{
  public Guid Id { get; set; }

  public DateTime Time { get; set; } = DateTime.Now;

  public string Title { get; set; }

  public string FirstType { get; set; }

  public Guid FirstId { get; set; }

  public IList<StackRow> Stack { get; set; } = new List<StackRow>();
}
