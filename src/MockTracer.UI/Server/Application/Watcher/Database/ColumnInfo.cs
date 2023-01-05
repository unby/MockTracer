using System.Text.Json.Serialization;

namespace MockTracer.UI.Server.Application.Watcher.Database;

public class ColumnInfo
{
  public int Index { get; set; }

  public string Name { get; set; }

  [JsonIgnore]
  public Type Type { get; set; }

  public string? TypeFullName { get; set; }
}
