using System.Data;
using Microsoft.Extensions.FileProviders;

namespace MockTracer.UI.Server;

/// <summary>
/// Assembly resource provider
/// </summary>
public class AssemblyResourceProvider : IFileProvider
{
  private readonly (string resourceName, string path)[] resources;

  /// <summary>
  /// index file info
  /// </summary>
  public static readonly (string resourceName, string path) Index = ("MockTracer.UI.Server.index.html", "/mocktracer/index.html");
  
  /// <summary>
  /// AssemblyResourceProvider
  /// </summary>
  public AssemblyResourceProvider()
  {
    this.resources = GetType().Assembly.GetManifestResourceNames()
      .Select(s => (s, "/mocktracer/" + s.Replace("MockTracer.UI.Server.", "").Replace("---", "/"))).ToArray();
  }

  /// <inheritdoc/>
  public IDirectoryContents GetDirectoryContents(string subpath)
  {
    return new MemoryDirectory(resources.Where(w => w.path.StartsWith(subpath)));
  }

  /// <inheritdoc/>
  public IFileInfo GetFileInfo(string subpath)
  {
    if (!Path.HasExtension(subpath))
    {
      return new AssemblyResourceFileInfo(Index.resourceName, subpath);
    }

    var resource = resources.First(f => f.path == subpath);
    return new AssemblyResourceFileInfo(resource.resourceName, subpath);
  }

  /// <inheritdoc/>
  public Microsoft.Extensions.Primitives.IChangeToken Watch(string filter)
  {
#pragma warning disable CS8603 // Possible null reference return.
    return null;
#pragma warning restore CS8603 // Possible null reference return.
  }
}
