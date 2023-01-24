namespace MockTracer.UI.Server.Application.Watcher;

public interface IScopeWatcher
{
  void AddInputAsync(TraceInfo trace, params ArgumentObjectInfo[]? serviceData);
  void AddOutputAsync(TraceInfo trace, ArgumentObjectInfo? serviceData);
  void Catch(TraceInfo trace, Exception ex);
}