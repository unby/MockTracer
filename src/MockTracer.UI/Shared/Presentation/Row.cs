using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Shared.Presentation;
public class Row
{
  public bool IsInput { get; set; }

  public bool IsOutput { get; set; }

  public List<Guid> ParentList { get; set; }

  public StackRow Stack { get; set; }
}
