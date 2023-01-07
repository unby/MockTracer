using Microsoft.Extensions.FileProviders;

namespace MockTracer.UI.Server;

public class MemoryFileInfo : IFileInfo
{
  private MemoryStream ms;
  private bool _disposedValue;

  public static MemoryFileInfo Index { get { return new MemoryFileInfo(ResourceProvider.Index.resourceName, ResourceProvider.Index.path); } }

  public MemoryFileInfo(string resourceName, string path)
  {
    Name = resourceName;
    Path = path;

    using var stream = GetType().Assembly.GetManifestResourceStream(Name);
    stream.Position = 0;
    byte[] buffer = new byte[16 * 1024];
    ms = new MemoryStream();

    int read;
    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
    {
      ms.Write(buffer, 0, read);
    }
    ms.Position = 0;
    Length = stream.Length;
  }

  public bool Exists => true;

  public bool IsDirectory => false;

  public DateTimeOffset LastModified => DateTime.Today;

  public long Length { get; }

  public string Name { get; }

  public string Path { get; }

  public string PhysicalPath => string.Empty;

  public Stream CreateReadStream()
  {
    var temp = ms;
    ms = null;
    return temp;
  }
}
