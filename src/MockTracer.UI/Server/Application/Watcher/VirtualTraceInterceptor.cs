using Castle.DynamicProxy;
using MockTracer.UI.Server.Application.Common;

namespace MockTracer.UI.Server.Application.Watcher;

/// <summary>
/// Custom tracer for interfaces or class with virtual methods
/// </summary>
public class VirtualTraceInterceptor : IInterceptor,  ITracer
{
  private readonly ScopeWatcher _scopeWatcher;

  /// <summary>
  /// VirtualTraceInterceptor
  /// </summary>
  /// <param name="scopeWatcher"><see cref="ScopeWatcher"/></param>
  public VirtualTraceInterceptor(ScopeWatcher scopeWatcher)
  {
    _scopeWatcher = scopeWatcher;
  }
  /// <inheritdoc/>
  public TraceInfo CreateInfo(string title)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.Custom,
    };
  }
  /// <inheritdoc/>
  public void Intercept(IInvocation invocation)
  {
    var info = CreateInfo(invocation.Proxy.GetType().Name + invocation.Method.Name);
    try
    {
      var method = invocation.Method.GetParameters();
      var inputs = invocation.Arguments.Select((s, i) => CustomTracer.ResolveArgument(s, method[i].ParameterType, method[i].Name ?? $"inArg{i}")).ToArray();
      _scopeWatcher.AddInputAsync(info, inputs);
      invocation.Proceed();
      _scopeWatcher.AddOutputAsync(
        info,
        invocation.ReturnValue != null ? CustomTracer.ResolveArgument(invocation.ReturnValue, invocation.ReturnValue.GetType(), "result") : null);
    }
    catch(Exception ex)
    {
      _scopeWatcher.Catch(info, ex);
    }
  }
}
