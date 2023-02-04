using System.Collections;
using System.Data;
using Microsoft.Extensions.FileProviders;

namespace MockTracer.UI.Server;

public class MemoryDirectory : IDirectoryContents
{
  private readonly (string resourceName, string path)[] _resources;

  public MemoryDirectory(IEnumerable<(string resourceName, string path)> resources)
  {
    Exists = resources.Any();
    _resources = resources.ToArray();
  }

  public bool Exists { get; }

  public IEnumerator<IFileInfo> GetEnumerator()
  {
    foreach (var item in _resources.Select(s => new AssemblyResourceFileInfo(s.resourceName, s.path)))
    {
      yield return item;
    }
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    foreach (var item in _resources.Select(s => new AssemblyResourceFileInfo(s.resourceName, s.path)))
    {
      yield return item;
    }
  }
}
