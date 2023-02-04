namespace MockTracer.UI.Server.Options;

/// <summary>
/// HTTP access
/// </summary>
public class HttpRouteOptions
{
  /// <summary>
  /// controller API routes
  /// </summary>
  public string[] Allows { get; set; } = new string[0];

  /// <summary>
  /// simple http route (without ControllerBase handler)
  /// </summary>
  public string[] AllowRequests { get; set; } = new string[0];

  /// <summary>
  /// Check controller's routes
  /// </summary>
  /// <param name="path">route</param>
  public bool IsWatch(string path)
  {
    return Allows.Contains("*") || Allows.Any(a => path.StartsWith(a));
  }

  /// <summary>
  /// Check controller's routes
  /// </summary>
  /// <param name="path">route</param>
  public bool IsHTTPWatch(string path)
  {
    return AllowRequests.Any(a => path.StartsWith(a));
  }
}
