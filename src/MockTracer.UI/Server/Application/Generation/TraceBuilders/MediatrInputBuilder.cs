using System.Globalization;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.TraceBuilders;

/// <summary>
/// Mediatr input point
/// </summary>
public class MediatrInputBuilder : InputPointBuilderBase
{
  /// <summary>
  /// MediatrInputBuilder
  /// </summary>
  /// <param name="nameReslover"><see cref="VariableNameReslover"/></param>
  public MediatrInputBuilder(VariableNameReslover nameReslover) : base(nameReslover)
  {
  }

  /// <inheritdoc/>
  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {
    var result = new List<LineFragment>();
    try
    {
      var input = row.Input?.FirstOrDefault();

      result.Add(BuildingConstans.Action.Line(@$"var mediator = host.GetInstance<IMediator>();"));

      if (row.Output != null)
      {
        if (row.Output.ClassName.EndsWith("Task"))
        {
          result.Add(BuildingConstans.Action.Line(@$"await mediator.Send({input.SharpCode});"));
        }
        else
        {
          result.Add(BuildingConstans.Action.Line(@$"var result = await mediator.Send({input.SharpCode});"));
          result.Add(BuildingConstans.Assert.Line(@$"/* Assert.Equal({row.Output.SharpCode}, result); */"));
        }
      }
      else if(row.Exception!=null)
      {
        result.Add(BuildingConstans.Assert.Line(@$"Assert.Throws<{row.Exception.ClassName}>(() => await mediator.Send({input.SharpCode}));"));
      }

    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Assert.Line("// Faild assert block", ex));
    }

    return result;
  }
}
