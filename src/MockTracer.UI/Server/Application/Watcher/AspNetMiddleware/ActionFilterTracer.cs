using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Options;

namespace MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;

/// <inheritdoc/>
public class ActionFilterTracer : IAsyncActionFilter, ITracer
{
  private readonly ScopeWatcher _scopeStore;
  private readonly MockTracerOption _options;

  /// <summary>
  /// ActionFilterTracer
  /// </summary>
  /// <param name="scopeStore"><see cref="ScopeWatcher"/></param>
  /// <param name="options"><see cref="MockTracerOption"/></param>
  public ActionFilterTracer(ScopeWatcher scopeStore, IOptions<MockTracerOption> options)
  {
    _scopeStore = scopeStore;
    _options = options.Value;
  }

  /// <inheritdoc/>
  public TraceInfo CreateInfo(string title, Type? type = null, MethodInfo? methodInfo = null)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.MvcActionFilter,
      CalledMethod = methodInfo,
      CalledType = type,
    };
  }

  /// <inheritdoc/>
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    if (context.Controller is ControllerBase controller && _options.AllowRoutes.IsWatch(controller.HttpContext.Request.Path))
    {
      var request = controller.HttpContext.Request;
      var n = context.ActionArguments.Keys;
      var obj = context.ActionArguments.Select(s => s.Value).FirstOrDefault();
      var httpRequest = new TraceHttpRequest(request.Path, $"{request.Path}{request.QueryString}".Trim('?'), request.ContentType, request.Method);

      var traceInfo = CreateInfo(httpRequest.FullPath);
      _scopeStore.AddInputAsync(traceInfo, new ArgumentObjectInfo()
      {
        ArgumentName = "request",
        Namespace = obj?.GetType().Namespace ?? string.Empty,
        ClassName = obj?.GetType().GetRealTypeName() ?? string.Empty,
        OriginalObject = obj,
        AdvancedInfo = httpRequest
      });

      var result = await next();

      if (result.Exception != null)
      {
        _scopeStore.Catch(traceInfo, result.Exception);
      }
      else
      {
        var httpResponse = new TraceHttpReponse
        {
          ContentType = controller.HttpContext.Response.ContentType,
          StatusCode = controller.HttpContext.Response.StatusCode
        };
        object? objResponse = null;

        if (result.Result is ObjectResult objectResult)
        {
          objResponse = objectResult.Value;
        }

        _scopeStore.AddOutputAsync(traceInfo, new ArgumentObjectInfo()
        {
          ArgumentName = "request",
          Namespace = objResponse?.GetType().Namespace ?? string.Empty,
          ClassName = objResponse?.GetType().GetRealTypeName() ?? string.Empty,
          OriginalObject = objResponse,
          AdvancedInfo = httpResponse
        });
      }
    }
    else
    {
      await next();
    }
  }
}
