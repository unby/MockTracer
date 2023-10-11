using System.Globalization;
using System.Net;
using System.Text.Json;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Server.Application.Watcher;
using MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.TraceBuilders;

/// <summary>
/// http client call
/// </summary>
public class MvcFilterBuilder : InputPointBuilderBase
{
  private static readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;
  private static readonly string[] AccesMethod = new string[] { "POST", "PATCH" };

  /// <summary>
  /// MvcFilterBuilder
  /// </summary>
  /// <param name="nameReslover"><see cref="VariableNameReslover"/></param>
  public MvcFilterBuilder(VariableNameReslover nameReslover) : base(nameReslover)
  {
  }

  /// <inheritdoc/>
  public override IEnumerable<LineFragment> BuildFragments(StackRow row)
  {
    var result = new List<LineFragment>();
    result.Add(BuildingConstans.Using.Line("using System.Net;"));
    try
    {
      var data = row.Input?.FirstOrDefault();
      var request = JsonSerializer.Deserialize<TraceHttpRequest>(data.AddInfo, ScopeWatcher.JsonOptions);

      var argument = string.Empty;

      if (AccesMethod.Contains(request.Method) && row.Input?.FirstOrDefault() is Input input)
      {
        argument = $", {ResolveName(input, result)}.ToHttpContent()";
      }

      result.Add(BuildingConstans.Action.Line(@$"var httpResult = await host.GetHttpClient().{_textInfo.ToTitleCase(request.Method.ToLower())}Async(""{request.FullPath}""{argument});"));
    }
    catch (Exception ex)
    {
      result.Add(BuildingConstans.Assert.Line("// Faild assert block", ex));
    }

    if (row.Output != null)
    {
      try
      {
        var response = JsonSerializer.Deserialize<TraceHttpReponse>(row.Output.AddInfo);
        var status = (HttpStatusCode)response.StatusCode;
        result.Add(BuildingConstans.Assert.Line($"Assert.Equal(HttpStatusCode.{status}, httpResult.StatusCode);"));
      }
      catch (Exception ex)
      {
        result.Add(BuildingConstans.Assert.Line("// Faild assert block", ex));
      }

      try
      {

        if (row.Output?.SharpCode != null)
        {
          var type = row.Output.FullName.FindType();
          if (type == null)
          {
            result.Add(BuildingConstans.Assert.Line($"// Unknown type {row.Output.FullName}"));
          }
          else if (type.IsClass)
          {
            result.Add(BuildingConstans.Assert.Line($"Assert.Equal({row.Output.SharpCode}, httpResult.ReadJson<{type.GetRealTypeName()}>());"));
          }
          else
          {
            result.Add(BuildingConstans.Assert.Line($"Assert.Equal(\"{row.Output.SharpCode}\", httpResult.Content.ReadAsStringAsync().Result);"));
          }
        }
      }
      catch (Exception ex)
      {
        result.Add(BuildingConstans.Assert.Line("// Faild assert block", ex));
      }
    }
    else if (row.Exception != null)
    {
      result.Add(BuildingConstans.Assert.Line($"Assert.NotEqual(HttpStatusCode.OK, httpResult.StatusCode);"));
    }

    return result;
  }
}
