using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Generation.MockBuilders;

public class HttpContextMockBuilder : MockBuilderBase
{
  public HttpContextMockBuilder(VariableNameReslover nameReslover)
    : base(nameReslover)
  {
  }

  public override IEnumerable<LineFragment> BuildFragments(StackRow input)
  {
    var result = new List<LineFragment>();
    return result;
  }
}
