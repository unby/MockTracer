using MockTracer.UI.Server.Application.Generation.Common;

namespace MockTracer.UI.Server.Application.Generation;

public abstract class MockPointBuilderBase : BuilderBase
{
  protected MockPointBuilderBase(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }
}
