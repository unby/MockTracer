using System.Reflection;
using Castle.DynamicProxy;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;

namespace MockTracer.UI.Server.Application.Watcher;

/// <summary>
/// Custom tracer for interfaces or class with virtual methods
/// </summary>
public class VirtualTraceInterceptor : IInterceptor, ITracer
{
  private readonly ScopeWatcher _scopeWatcher;
  private Type _type;

  /// <summary>
  /// VirtualTraceInterceptor
  /// </summary>
  /// <param name="scopeWatcher"><see cref="ScopeWatcher"/></param>
  /// <param name="type">origin type</param>
  public VirtualTraceInterceptor(ScopeWatcher scopeWatcher, Type type)
  {
    _scopeWatcher = scopeWatcher;
    _type = type;
  }

  /// <inheritdoc/>
  public TraceInfo CreateInfo(string title, Type? type = null, MethodInfo? methodInfo = null)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.Custom,
      CalledType = type,
      CalledMethod = methodInfo,
    };
  }

  /// <inheritdoc/>
  public void Intercept(IInvocation invocation)
  {
    var info = CreateInfo(_type.Namespace + "=>" + _type.Name + "=>" + invocation.Method.GetRealMethodName(), _type, invocation.Method);
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
    catch (Exception ex)
    {
      _scopeWatcher.Catch(info, ex);
    }
  }
}
