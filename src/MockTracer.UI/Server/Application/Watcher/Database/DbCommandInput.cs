using System.Data;

namespace MockTracer.UI.Server.Application.Watcher.Database;

public sealed class DbCommandInput
{
  public List<MockParameter> Parameters { get; set; } = new List<MockParameter>();

  public CommandType CommandType { get; set; }

  public string CommandText { get; set; }
}
