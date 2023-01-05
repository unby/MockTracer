using MockTracer.UI.Server.Application.Generation.Common;

namespace MockTracer.UI.Server.Application.Generation;

public abstract class MockBuilderBase : BuilderBase
{
  protected MockBuilderBase(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }
}
