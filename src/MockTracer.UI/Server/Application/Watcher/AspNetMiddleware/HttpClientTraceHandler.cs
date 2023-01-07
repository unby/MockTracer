using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Presentation;

namespace MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;

public class HttpClientTraceHandler : DelegatingHandler, ITracer
{
  private readonly ScopeWathcer _scopeStore;

  public HttpClientTraceHandler(IHttpContextAccessor accessor)
      : base()
  {
    _scopeStore = accessor.HttpContext.RequestServices.GetService<ScopeWathcer>();
  }

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var traceInfo = MakeInfo(request.RequestUri.AbsolutePath);
    await _scopeStore.AddInputAsync(traceInfo, new ServiceData()
    {
      ArgumentName = "request",
      ClassName = request.GetType().GetRealTypeName(),
      Namespace = request.GetType().Namespace,
      OriginalObject = request.Content != null ? await request.Content.ReadAsStringAsync() : null,
      AdvancedInfo = new TraceHttpRequest()
      {
        FullPath = request.RequestUri.ToString(),
        Path = request.RequestUri.AbsolutePath,
        ContentType = request.Content?.Headers?.ContentType?.MediaType.ToString(),
        Method = request.Method.ToString()
      }
    });
    try
    {
      HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

      var result = await ResolveResponseAsync(response, traceInfo);
      await _scopeStore.AddOutputAsync(traceInfo, new ServiceData()
      {
        ArgumentName = "response",
        ClassName = result.className,
        Namespace = result.Namespace,
        OriginalObject = result.obj,
        AdvancedInfo = new TraceHttpReponse()
        {
          StatusCode = (int)response.StatusCode,
          ContentType = response.Content?.Headers?.ContentType?.MediaType.ToString()
        }
      });
      return response;
    }
    catch (Exception ex)
    {
      await _scopeStore.Catch(traceInfo, ex);
      throw;
    }
  }

  public TraceInfo MakeInfo(string title)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.HttpClient
    };
  }

  private async Task<(string className, string? Namespace, object? obj)> ResolveResponseAsync(HttpResponseMessage response, TraceInfo traceInfo)
  {
    var result = string.Empty;
    if (response.Content == null)
    {
      return (result.GetType().GetRealTypeName(), result.GetType().Namespace, result);
    }

    try
    {
      var type = traceInfo.StackTrace.FirstOrDefault(w => w.DeclaringTypeNamespace.StartsWith("Refit.Implementation"));
      result = await response.Content.ReadAsStringAsync();
      var objType = type.OutputTypeName.Replace("Task<", string.Empty).TrimEnd('>').FindType();

      if (objType == null)
      {
        return (result.GetType().Name, result.GetType().Namespace, result);
      }
      else
      {
        object obj = JsonSerializer.Deserialize(result, objType, ScopeWathcer.JsonOptions);
        return (objType.Name, objType.Namespace, obj);
      }
    }
    catch
    {
      return (result.GetType().Name, result.GetType().Namespace, result);
    }
  }
}
