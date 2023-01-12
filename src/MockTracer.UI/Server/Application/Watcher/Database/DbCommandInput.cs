using System.Data;

namespace MockTracer.UI.Server.Application.Watcher.Database;

/// <summary>
/// SQL data comand agreggate
/// </summary>
public sealed class DbCommandInput
{
  /// <summary>
  /// SQL parameters
  /// </summary>
  public List<MockParameter> Parameters { get; set; } = new List<MockParameter>();

  /// <summary>
  /// Command type
  /// </summary>
  public CommandType CommandType { get; set; }

  /// <summary>
  /// Command text
  /// </summary>
  public string CommandText { get; set; }
}
