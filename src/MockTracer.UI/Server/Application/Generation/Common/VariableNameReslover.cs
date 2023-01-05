namespace MockTracer.UI.Server.Application.Generation.Common;

public class VariableNameReslover
{
  Dictionary<string, List<string>> VariableValues = new Dictionary<string, List<string>>();

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
