using MockTracer.UI.Server.Application.Generation.Common;

namespace MockTracer.UI.Server.Application.Generation;

/// <summary>
/// Start point
/// </summary>
public abstract class InputPointBuilderBase
  : BuilderBase
{
  /// <summary>
  /// InputPointBuilderBase
  /// </summary>
  /// <param name="nameReslover"><see cref="VariableNameReslover"/></param>
  protected InputPointBuilderBase(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }
}
