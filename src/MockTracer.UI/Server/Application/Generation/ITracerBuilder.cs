using MockTracer.UI.Server.Application.Generation.Common;

namespace MockTracer.UI.Server.Application.Generation;

public abstract class TracerBuilderBase
  : BuilderBase
{
  protected TracerBuilderBase(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }
}
