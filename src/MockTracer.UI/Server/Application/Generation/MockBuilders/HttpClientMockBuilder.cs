using System.Net;
using System.Text.Json;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Server.Application.Watcher;
using MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.MockBuilders;

public class HttpClientMockBuilder : MockBuilderBase
{
  public HttpClientMockBuilder(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }

  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {

    var result = new List<LineFragment>();
    try
    {
      var input = row.Input.First();

      var request = JsonSerializer.Deserialize<TraceHttpRequest>(input.AddInfo, ScopeWatcher.JsonOptions);
      var response = JsonSerializer.Deserialize<TraceHttpReponse>(row.Output.AddInfo, ScopeWatcher.JsonOptions);
      var status = (HttpStatusCode)response.StatusCode;
      //  HttpMethod.Get TraceHttpReponse TraceHttpRequest
      var output = string.Empty; 

      var variable = NameReslover.CheckName($"httpClient");
      var withMediaType = string.Empty;
      if (response.ContentType != null)
      {
        withMediaType = $".WithMediaType(\"{response.ContentType}\")";
      }
      var withContent = string.Empty;
      if (row.Output.SharpCode != null)
      {
        var httpResult = ResolveName(row.Output, result);
        if (row.Output.ClassName.EndsWith("String", StringComparison.OrdinalIgnoreCase))
        {
          output= $"{Environment.NewLine}.WithContent(() => Encoding.UTF8.GetBytes({httpResult}))";
        }
        else
        {
          output = $"{Environment.NewLine}.WithContent(() => {httpResult}.ToUtf8Bytes())";
        }
      }

      result.Add(BuildingConstans.Prepare.Line($@" var {variable} = new HttpRequestInterceptionBuilder().Requests().For(f=> f.Method == HttpMethod.{request.Method.ToTitle()} && f.RequestUri.AbsolutePath.EndsWith(""{request.Path}""))
               .Responds(){withMediaType}
              {output}
               .WithStatus(HttpStatusCode.{status})
               .RegisterWith(HttpClientInterceptor);"));
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Prepare.Line("// faild prepare", ex));
    }

    return result;
  }
}
