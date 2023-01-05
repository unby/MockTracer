using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MockTracer.UI.Client.Shared
{
  public class AppRouteView : RouteView
  {
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    protected override void Render(RenderTreeBuilder builder)
    {
      base.Render(builder);
    }
  }
}
