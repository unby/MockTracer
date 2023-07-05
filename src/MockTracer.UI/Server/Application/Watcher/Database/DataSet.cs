using System.Data;
using System.Text.Json;

namespace MockTracer.UI.Server.Application.Watcher.Database;

public class DataSet
{
  public Dictionary<int, ColumnInfo> Header { get; set; } = new Dictionary<int, ColumnInfo>();

  public List<object?[]> Data { get; set; } = new List<object?[]>();

  internal object?[] Row;

  public int GetIndexByName(string name)
  {
    return Header.First(f => f.Value.Name == name).Key;
  }

  internal void NextRow()
  {
    Row = new object?[Header.Count];
    Data.Add(Row);
  }
  internal void RefreshtRow()
  {
    Data.Clear();
    Row = new object?[Header.Count];
    Data.Add(Row);
  }

  public static string[] Parse(object?[] row)
  {
    return row.Cast<JsonElement>().Select(s => s.GetRawText() ?? "null").ToArray();
  }
}
