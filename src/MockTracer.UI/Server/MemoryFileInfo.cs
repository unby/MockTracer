using Microsoft.Extensions.FileProviders;

namespace MockTracer.UI.Server;

/// <inheritdoc/>
public class MemoryFileInfo : IFileInfo
{
  private MemoryStream? ms;

  /// <summary>
  /// index.html file
  /// </summary>
  public static MemoryFileInfo Index { get { return new MemoryFileInfo(ResourceProvider.Index.resourceName, ResourceProvider.Index.path); } }

  /// <summary>
  /// MemoryFileInfo
  /// </summary>
  /// <param name="resourceName">file name</param>
  /// <param name="path">resource name</param>
  public MemoryFileInfo(string resourceName, string path)
  {
    Name = resourceName;
    Path = path;
  }

  /// <inheritdoc/>
  public bool Exists => true;

  /// <inheritdoc/>
  public bool IsDirectory => false;

  /// <inheritdoc/>
  public DateTimeOffset LastModified => DateTime.Today;

  /// <inheritdoc/>
  public long Length { get; } = 1;

  /// <inheritdoc/>
  public string Name { get; }

  /// <inheritdoc/>
  public string Path { get; }

  /// <inheritdoc/>
  public string PhysicalPath => string.Empty;

  /// <inheritdoc/>
  public Stream CreateReadStream()
  {
      using var stream = GetType().Assembly.GetManifestResourceStream(Name);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      stream.Position = 0;
      byte[] buffer = new byte[16 * 1024];
      var ms = new MemoryStream();

      int read;
      while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
      {
        ms.Write(buffer, 0, read);
      }
      ms.Position = 0;
      return ms;
  }
}
