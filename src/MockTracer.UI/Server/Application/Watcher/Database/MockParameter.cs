using System.Data;

namespace MockTracer.UI.Server.Application.Watcher.Database;

public sealed class MockParameter
{
  public string Name { get; set; }

  public string Type { get; set; }

  public object? Value { get; set; }

  public ParameterDirection ParameterDirection { get; set; }
}
