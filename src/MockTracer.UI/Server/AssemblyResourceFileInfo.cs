using Microsoft.Extensions.FileProviders;

namespace MockTracer.UI.Server;

/// <summary>
/// Wrapper about file from assembly resources
/// </summary>
public class AssemblyResourceFileInfo : IFileInfo
{
  private MemoryStream? _cache;

  /// <summary>
  /// index file
  /// </summary>
  public static AssemblyResourceFileInfo Index { get { return new AssemblyResourceFileInfo(AssemblyResourceProvider.Index.resourceName, AssemblyResourceProvider.Index.path); } }

  /// <summary>
  /// AssemblyResourceFileInfo
  /// </summary>
  /// <param name="resourceName">resource name</param>
  /// <param name="path">route path</param>
  public AssemblyResourceFileInfo(string resourceName, string path)
  {
    Name = resourceName;
    Path = path;
    if (path.Contains("index"))
    {
      Console.WriteLine("Ind");
    }

    using var stream = GetType().Assembly.GetManifestResourceStream(Name);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    stream.Position = 0;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    byte[] buffer = new byte[16 * 1024];
    _cache = new MemoryStream();

    int read;
    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
    {
      _cache.Write(buffer, 0, read);
    }
    _cache.Position = 0;
    Length = stream.Length;
  }

  /// <inheritdoc/>
  public bool Exists => true;

  /// <inheritdoc/>
  public bool IsDirectory => false;

  /// <inheritdoc/>
  public DateTimeOffset LastModified => DateTime.Today;

  /// <inheritdoc/>
  public long Length { get; }

  /// <inheritdoc/>
  public string Name { get; }

  /// <inheritdoc/>
  public string Path { get; }

  /// <inheritdoc/>
  public string PhysicalPath => string.Empty;

  /// <inheritdoc/>
  public Stream CreateReadStream()
  {
    var temp = _cache;
    _cache = null;
#pragma warning disable CS8603 // Possible null reference return.
    return temp;
#pragma warning restore CS8603 // Possible null reference return.
  }
}
