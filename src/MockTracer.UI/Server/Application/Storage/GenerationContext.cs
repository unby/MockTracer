using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Presentation;

public sealed class GenerationContext
{
  public StackRow Input { get; set; }

  public StackRow[] Output { get; set; } = new StackRow[0];
}
