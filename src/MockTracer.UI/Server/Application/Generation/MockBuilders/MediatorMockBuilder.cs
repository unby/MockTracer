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
    var result = new List<LineFragment>(8);
    try
    {
      result.AddNameSpace("MediatR", "Moq");
      var input = result.ResolveClassName(row.Input.First());
      var variable = NameReslover.CheckName($"{row.Input.FirstOrDefault().Name}MockObject");

      if (row.Output != null)
      {
        var output = result.ResolveClassName(row.Output);
        result.Add(BuildingConstans.Prepare.Line($"var {variable} = new Mock<IRequestHandler<{input}, {output}>>();"));
        result.Add(BuildingConstans.Prepare.Line($"{variable}.Setup(s => s.Handle(It.IsAny<{input}>(), It.IsAny<CancellationToken>())).ReturnsAsync({ResolveName(row.Output, result)});"));
      }
      else
      {
        // todo: убрать Task из row.OutputTypeName 
        result.Add(BuildingConstans.Prepare.Line($"var {variable} = new Mock<IRequestHandler<{input}, {row.OutputTypeName}>>();"));
        result.Add(BuildingConstans.Prepare.Line($"{variable}.Setup(s => s.Handle(It.IsAny<{input}>(), It.IsAny<CancellationToken>())).Throws({row.Exception.SharpCode}));"));
      }
      result.Add(BuildingConstans.Configure.Line($"s.Replace({variable})"));
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Prepare.Line("// faild prepare", ex));
    }

    return result;
  }
}
