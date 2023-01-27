namespace MockTracer.UI.Server.Application.Generation.Common;

/// <summary>
/// store with 
/// </summary>
public class VariableNameReslover
{
  private Dictionary<string, List<string>> VariableValues = new Dictionary<string, List<string>>();

  /// <summary>
  /// Resolve unique member name
  /// </summary>
  /// <param name="name">desired name</param>
  /// <returns>free name</returns>
  public string CheckName(string name)
  {
    if (VariableValues.TryGetValue(name, out var values))
    {
      var last = values.Last();
      if (last.Equals(name))
      {
        last = name + "2";
      }
      else
      {
        last = name + (1 + int.Parse(last.Replace(name, string.Empty))).ToString();
      }

      values.Add(last);
      return last;
    }
    else
    {
      VariableValues.Add(name, new List<string>() { name });
      return name;
    }
  }
}
