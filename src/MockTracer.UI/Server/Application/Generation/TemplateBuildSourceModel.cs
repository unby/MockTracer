using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Application.Generation;

public class TemplateBuildSourceModel
{
  public StackScope StackScope { get; init; }

  public StackRow Input { get; init; }

  public StackRow[] Output { get; init; }

  public GenerationAttributes Attributes { get; init; }
}
