using System.Text;
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
    var result = new List<LineFragment>();
    try
    {
      var input = row.Input?.FirstOrDefault();
      if (!string.IsNullOrEmpty(row.DeclaringTypeNamespace))
      { result.Add(BuildingConstans.Using.Line(row.DeclaringTypeNamespace)); }
      var instanceVar = NameReslover.CheckName(row.DeclaringTypeName.TrimStart('I'));
      result.Add(BuildingConstans.Action.Line(@$"var {instanceVar} = host.GetInstance<{row.DeclaringTypeName}>();"));
      var resultVar = NameReslover.CheckName("result");
      var args = new StringBuilder();
      bool appendComma = false;
      foreach (var item in row.Input)
      {
        if (appendComma) args.Append(',');
        args.Append(item.SharpCode);
        appendComma = true;
      }

      if(row.OutputTypeName?.Equals("Task") ?? false)
      {
        result.Add(BuildingConstans.Action.Line(@$"await {instanceVar}.{row.MethodName}({args});"));
      }
      if (!string.IsNullOrEmpty(row.OutputTypeName))
      {
        result.Add(BuildingConstans.Action.Line(@$"var {resultVar} = {(row.OutputTypeName.StartsWith("Task<") ? "await" : string.Empty)} {instanceVar}.{row.MethodName}({args});"));
        result.Add(BuildingConstans.Assert.Line(@$"/*
Assert.Equal({row.Output.SharpCode}, {resultVar});
*/"));
      }
      else
      {
        result.Add(BuildingConstans.Action.Line(@$"{instanceVar}.{row.MethodName}({args});"));
      }
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Assert.Line("// Faild assert block", ex));
    }

    return result;
  }
}
