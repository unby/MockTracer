using Microsoft.Extensions.Hosting;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.TraceBuilders;

/// <summary>
/// Custom interface input point
/// </summary>
public class CustomInputBuilder : InputPointBuilderBase
{
  /// <summary>
  /// CustomInputBuilder
  /// </summary>
  /// <param name="nameReslover"><see cref="VariableNameReslover"/></param>
  public CustomInputBuilder(VariableNameReslover nameReslover) : base(nameReslover)
  {
  }

  /// <inheritdoc/>
  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {
    /*
    var dataSource = host.GetInstance<IDataSource>();
    var result = dataSource.MultupleQueryAsync(1, "12");
    */
    var result = new List<LineFragment>();
    try
    {
      var input = row.Input?.FirstOrDefault();

      result.Add(BuildingConstans.Action.Line(@$"var mediator = host.GetInstance<IMediator>();"));


      if (row.Output.ClassName.EndsWith("Task"))
      {
        result.Add(BuildingConstans.Action.Line(@$"await mediator.Send({input.SharpCode});"));
      }
      else
      {
        result.Add(BuildingConstans.Action.Line(@$"var result = await mediator.Send({input.SharpCode});"));
        result.Add(BuildingConstans.Assert.Line(@$"/*
Assert.Equal({row.Output.SharpCode}, result);
*/"));
      }

    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Assert.Line("// Faild assert block", ex));
    }

    return result;
  }
}
