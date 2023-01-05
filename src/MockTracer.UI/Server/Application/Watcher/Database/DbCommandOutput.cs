namespace MockTracer.UI.Server.Application.Watcher.Database;

public sealed class DbCommandOutput
{
  public List<MockParameter> Parameters { get; set; } = new List<MockParameter>();
  public int ExecuteNonQueryResult { get; set; }
}
