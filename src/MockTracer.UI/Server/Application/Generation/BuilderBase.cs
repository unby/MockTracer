using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation;

public abstract class BuilderBase
{
  protected BuilderBase(VariableNameReslover nameReslover)
  {
    NameReslover = nameReslover;
  }

  public abstract IEnumerable<LineFragment> BuildFragments(StackRow row);

  protected string ResolveName(TracedObject obj, List<LineFragment> result)
  {
    if (CountLines(obj.SharpCode) > 12)
    {
      var variableName = NameReslover.CheckName(obj.Name);
      var className = result.ResolveClassName(obj);

      result.Add(BuildingConstans.BigVariable.Line($"  private {className} {variableName} = {obj.SharpCode};"));
      return variableName;
    }

    return obj.SharpCode;
  }

  protected string ResolveName(string str, List<LineFragment> result, string variableName)
  {
    if (CountLines(str) > 12)
    {
      variableName = NameReslover.CheckName(variableName);
      
      result.Add(BuildingConstans.BigVariable.Line($"  private string {variableName} = \"{str}\";"));
      return variableName;
    }

    return $"\"{str}\"";
  }

  protected VariableNameReslover NameReslover { get; }

  private static int CountLines(string str)
  {
    if (string.IsNullOrEmpty(str))
    {
      return 0;
    }

    int index = -1;
    int count = 0;
    while (-1 != (index = str.IndexOf(Environment.NewLine, index + 1)))
    {
      count++;
    }

    return count + 1;
  }
}
