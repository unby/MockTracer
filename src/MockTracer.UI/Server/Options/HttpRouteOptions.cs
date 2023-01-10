namespace MockTracer.UI.Server.Options;

public class HttpRouteOptions
{
  public string[] Allows { get; set; } = new string[0];

  public bool IsWatch(string path)
  {
    return Allows.Contains("*") || Allows.Any(a => path.StartsWith(a));
  }
}
