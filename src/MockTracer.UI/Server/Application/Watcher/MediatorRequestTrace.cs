using MediatR;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;

namespace MockTracer.UI.Server.Application.Watcher;

public class MediatorRequestTrace<TRequest, TResponse>
    : ITracer,
      IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
  private readonly ScopeWathcer _traceStore;

  public MediatorRequestTrace(ScopeWathcer traceStore)
  {
    _traceStore = traceStore;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    var info = MakeInfo(request.GetType().FullName);
    try
    {
      var requestType = typeof(TRequest);
      await _traceStore.AddInputAsync(info, new ServiceData()
      {
        ArgumentName = nameof(request),
        ClassName = requestType.GetRealTypeName(),
        Namespace = requestType.Namespace,
        OriginalObject = request
      });
      var response = await next();
      var responseType = typeof(TResponse);
      await _traceStore.AddOutputAsync(info, new ServiceData()
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
      await _traceStore.Catch(info, ex);
      throw;
    }
  }

  public TraceInfo MakeInfo(string title)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.Mediatr
    };
  }
}
