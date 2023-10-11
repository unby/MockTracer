using System.Reflection;
using MediatR;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;

namespace MockTracer.UI.Server.Application.Watcher;

/// <summary>
/// Mediator behavior trace
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class MediatorRequestTrace<TRequest, TResponse>
    : ITracer,
      IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
  private readonly ScopeWatcher _traceStore;

  public MediatorRequestTrace(ScopeWatcher traceStore)
  {
    _traceStore = traceStore;
  }

  /// <inheritdoc/>
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    var requestType = typeof(TRequest);
    var responseType = typeof(TResponse);
    var info = CreateInfo(request.GetType().FullName, requestType, next.Method, responseType);
    try
    {
      _traceStore.AddInputAsync(info, new ArgumentObjectInfo()
      {
        ArgumentName = nameof(request),
        ClassName = requestType.GetRealTypeName(),
        Namespace = requestType.Namespace,
        OriginalObject = request
      });
      var response = await next();
     
      _traceStore.AddOutputAsync(info, new ArgumentObjectInfo()
      {
        ArgumentName = nameof(response),
        ClassName = responseType.GetRealTypeName(),
        Namespace = responseType.Namespace,
        OriginalObject = response
      });
      return response;
    }
    catch (Exception ex)
    {
      _traceStore.Catch(info, ex);
      throw;
    }
  }

  /// <inheritdoc/>
  public TraceInfo CreateInfo(string title, Type? type = null, MethodInfo? methodInfo = null, Type? outputType = null)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.Mediatr,
      CalledMethod = methodInfo,
      CalledType = type,
    };
  }
}
