using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Options;

namespace MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;

public class HttpContextTrace : ITracer
{
  private readonly RequestDelegate _next;
  private readonly HttpRouteOptions _options;

  public HttpContextTrace(RequestDelegate next, IOptions<MockTracerOption> options)
  {
    _next = next;
    _options = options.Value.AllowRoutes;
  }

  public async Task Invoke(HttpContext context, ScopeWathcer _scopeStore)
  {
    if (_options.IsWatch(context.Request.Path))
    {
      var request = await GetRequestAsync(context.Request);
      var traceInfo = MakeInfo(request.req.FullPath);
      await _scopeStore.AddInputAsync(traceInfo, new ServiceData()
      {
        ArgumentName = "request",
        Namespace = context.Request.GetType().Namespace,
        ClassName = context.Request.GetType().GetRealTypeName(),
        OriginalObject = request.body,
        AdvancedInfo = request.req
      });
      var originalBodyStream = context.Response.Body;

      using (var responseBody = new MemoryStream())
      {
        context.Response.Body = responseBody;
        try
        {
          var data = context.GetEndpoint();

          await _next(context);
          var value = context.GetRouteData();
        }
        catch (Exception ex)
        {
          await _scopeStore.Catch(traceInfo, ex);
          throw;
        }

        var response = await GetResponseAsTextAsync(context.Response);
        await _scopeStore.AddOutputAsync(traceInfo, new ServiceData()
        {
          ArgumentName = "response",
          ClassName = context.Response.GetType().GetRealTypeName(),
          Namespace = context.Response.GetType().Namespace,
          OriginalObject = response.body,
          AdvancedInfo = response.response
        });
        await responseBody.CopyToAsync(originalBodyStream);
      }
    }
    else
    {
      await _next(context);
    }
  }

  public TraceInfo MakeInfo(string title)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.HttpContext
    };
  }

  private async Task<(TraceHttpRequest req, string? body)> GetRequestAsync(HttpRequest request)
  {
    string? bodyAsText = null;
    request.EnableBuffering();
    if (request.ContentLength > 0)
    {
      var buffer = new byte[Convert.ToInt32(request.ContentLength)];
      await request.Body.ReadAsync(buffer, 0, buffer.Length);
      bodyAsText = Encoding.UTF8.GetString(buffer);
      var temp = request.Body;
      request.Body = new MemoryStream(buffer);
      temp.Dispose();
    }
    request.Body.Seek(0, SeekOrigin.Begin);
    string path = $"{request.Path}{(!string.IsNullOrEmpty(request.QueryString.ToString()) ? "?" + request.QueryString : string.Empty)}";
    return (new TraceHttpRequest() { Method = request.Method, ContentType = request.ContentType, FullPath = path, Path = request.Path }, bodyAsText);
  }

  private async Task<(TraceHttpReponse response, string? body)> GetResponseAsTextAsync(HttpResponse response)
  {
    response.Body.Seek(0, SeekOrigin.Begin);
    using var reader = new StreamReader(response.Body, leaveOpen: true);
    var text = await reader.ReadToEndAsync();
    response.Body.Seek(0, SeekOrigin.Begin);

    return (new TraceHttpReponse { ContentType = response.ContentType, StatusCode = response.StatusCode }, text);
  }
}
