namespace MockTracer.UI.Server.Options;

public class HttpRouteOptions
{
  public string[] Allows { get; set; } = new[] {"/api"};

  public bool IsWatch(string path)
  {
    return Allows.Any(a => path.StartsWith(a));
  }
}
