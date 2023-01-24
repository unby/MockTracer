using System.Globalization;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.TraceBuilders;

/// <summary>
/// Mediatr input point
/// </summary>
public class MediatrInputBuilder : InputPointBuilderBase
{
  private static readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;
  private static readonly string[] AccesMethod = new string[] { "POST", "PATCH" };

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
