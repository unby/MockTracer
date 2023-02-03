using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.MockBuilders;

/// <summary>
/// Mock virtual methods
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
      var variable = NameReslover.CheckName($"{row.DeclaringTypeName.TrimStart('I')}MockObject");
      result.Add(BuildingConstans.Prepare.Line($"var {variable} = new Mock<{row.DeclaringTypeName}>();"));
      string setResult = string.Empty;
      if (row.Output != null)
      {
        var output = result.ResolveClassName(row.Output);
        setResult = $".Returns(Task.FromResult<{output}>({ResolveName(row.Output, result)}))";
      }
      var args = row.Input?.Select(s => result.ResolveClassName(s)).Select(s => $"It.IsAny<{s}>()");
      result.Add(BuildingConstans.Prepare.Line($"{variable}.Setup(s => s.{row.MethodName}({(args != null ? string.Join(", ", args) : string.Empty)})){setResult};"));
      result.Add(BuildingConstans.Configure.Line($"s.Replace({variable})"));
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Prepare.Line($"// faild {nameof(CustomMockBuilder)} prepare", ex));
    }

    return result;
  }
}
