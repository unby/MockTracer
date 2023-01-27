using System.Text.RegularExpressions;
using SequentialGuid;

namespace MockTracer.UI.Server.Application.Common;

/// <summary>
/// Helper ext
/// </summary>
public static class VariableMaster
{
  private static readonly Regex nameClearRegex = new Regex(string.Format("[{0}]",  Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))));

  /// <summary>
  /// Generate id
  /// </summary>
  /// <returns>sequenced ID</returns>
  public static Guid Next()
  {
    return SequentialGuidGenerator.Instance.NewGuid();
  }

  /// <summary>
  /// Remove forbidden chars
  /// </summary>
  /// <param name="source">name</param>
  /// <returns>clear name</returns>
  public static string ClearFileName(this string source)
  {
    return nameClearRegex.Replace(source, "_").Replace("=", "_").Replace("&", "_").Replace("-", "_").Replace("__", "_");
  }
}
