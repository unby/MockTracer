using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.MockBuilders;

/// <summary>
/// Fake builder
/// </summary>
public class FakeMockBuilder : MockPointBuilderBase
{
  /// <summary>
  /// Fake builder
  /// </summary>
  /// <param name="nameReslover"><see cref="VariableNameReslover"/></param>
  public FakeMockBuilder(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }

  /// <inheritdoc/>
  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {
    return Enumerable.Empty<LineFragment>();
  }
}
