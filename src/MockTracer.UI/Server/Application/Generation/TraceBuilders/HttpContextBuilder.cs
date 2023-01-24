using System.Text.Json;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Server.Application.Watcher;
using MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.TraceBuilders;

public class HttpContextBuilder : InputPointBuilderBase
{
   public HttpContextBuilder(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }

  public override IEnumerable<LineFragment> BuildFragments(StackRow input)
  {
    var data = input.Input.FirstOrDefault();
    var request = JsonSerializer.Deserialize<TraceHttpRequest>(data.Json, ScopeWatcher.JsonOptions);

    var result = new List<LineFragment>();
    try
    {
      result.Add(new LineFragment(BuildingConstans.Action, @$"var httpResult = await host.GetHttpClient().{request.Method.ToTitle()}Async(""{request.FullPath}"");"));
    }
    catch (Exception)
    {
      result.Add(new LineFragment(BuildingConstans.Assert, "// Faild assert block"));
    }

    try
    {
      result.Add(new LineFragment(BuildingConstans.Assert, "Assert.Equal(HttpStatusCode.OK, httpResult.StatusCode);"));
    }
    catch (Exception)
    {
      result.Add(new LineFragment(BuildingConstans.Assert, "// Faild assert block"));
    }

    return result;
  }
}
