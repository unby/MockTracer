using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Watcher;

public class TraceInfo
{
  public Guid TraceId { get; init; }

  public StackCallItem[] StackTrace { get; init; } = FragmentExtention.ReadTrace();

  public string TracerType { get; init; }

  public string Title { get; init; }
}

