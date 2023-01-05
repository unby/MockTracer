namespace MockTracer.UI.Server.Application.Watcher;

public class ServiceData
{
  public string ArgumentName { get; init; }

  public object? OriginalObject { get; init; }

  public object? AdvancedInfo { get; init; }

  public string? Namespace { get; init; }

  public string ClassName { get; init; }
}

