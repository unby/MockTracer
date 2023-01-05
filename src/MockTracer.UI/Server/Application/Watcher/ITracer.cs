namespace MockTracer.UI.Server.Application.Watcher;

public interface ITracer
{
  TraceInfo MakeInfo(string title);
}
