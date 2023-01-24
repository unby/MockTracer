using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.MockBuilders;

/// <summary>
/// Mock IMediator handler
/// </summary>
public class MediatorMockBuilder : MockPointBuilderBase
{
  public MediatorMockBuilder(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }

  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {
    var result = new List<LineFragment>();
    try
    {
      var input = result.ResolveClassName(row.Input.First());
      var output = result.ResolveClassName(row.Output);
      var variable = NameReslover.CheckName($"{row.Input.FirstOrDefault().Name}MockObject");
      result.Add(BuildingConstans.Prepare.Line($"var {variable} = new Mock<IRequestHandler<{input}, {output}>>();"));
      result.Add(BuildingConstans.Prepare.Line($"{variable}.Setup(s => s.Handle(It.IsAny<{input}>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<{output}>({ResolveName(row.Output, result)}));"));

      result.Add(BuildingConstans.Configure.Line($"s.Replace({variable})"));
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Prepare.Line("// faild prepare", ex));
    }

    return result;
  }
}

/// <summary>
/// Mock custom interface
/// </summary>
public class CustomMockBuilder : MockPointBuilderBase
{
  /// <summary>
  /// CustomMockBuilder
  /// </summary>
  /// <param name="nameReslover"><see cref="VariableNameReslover"/></param>
  public CustomMockBuilder(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }

  /// <inheritdoc/>
  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {
    var result = new List<LineFragment>();
    try
    {
      var input = result.ResolveClassName(row.Input.First());
      var output = result.ResolveClassName(row.Output);
      var variable = NameReslover.CheckName($"{row.Input.FirstOrDefault().Name}MockObject");
      result.Add(BuildingConstans.Prepare.Line($"var {variable} = new Mock<IRequestHandler<{input}, {output}>>();"));
      result.Add(BuildingConstans.Prepare.Line($"{variable}.Setup(s => s.Handle(It.IsAny<{input}>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<{output}>({ResolveName(row.Output, result)}));"));

      result.Add(BuildingConstans.Configure.Line($"s.Replace({variable})"));
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Prepare.Line("// faild prepare", ex));
    }

    return result;
  }
}
