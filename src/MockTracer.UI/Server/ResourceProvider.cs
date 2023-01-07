using System.Data;
using Microsoft.Extensions.FileProviders;

namespace MockTracer.UI.Server;

public class ResourceProvider : IFileProvider
{
  private readonly (string resourceName, string path)[] resources;
  public static readonly (string resourceName, string path) Index = ("MockTracer.UI.Server.index.html", "/mocktracer/index.html");
  public ResourceProvider()
  {
    this.resources = GetType().Assembly.GetManifestResourceNames()
      .Select(s => (s, "/mocktracer/" + s.Replace("MockTracer.UI.Server.", "").Replace("---", "/"))).ToArray();
  }

  public IDirectoryContents GetDirectoryContents(string subpath)
  {
    return new MemoryDirectory(resources.Where(w => w.path.StartsWith(subpath)));
  }

  public IFileInfo GetFileInfo(string subpath)
  {
    if (!Path.HasExtension(subpath))
    {
      return new MemoryFileInfo(Index.resourceName, subpath);
    }

    var resource = resources.First(f => f.path == subpath);
    return new MemoryFileInfo(resource.resourceName, subpath);
  }

  public Microsoft.Extensions.Primitives.IChangeToken Watch(string filter)
  {
#pragma warning disable CS8603 // Possible null reference return.
    return null;
#pragma warning restore CS8603 // Possible null reference return.
  }
}
