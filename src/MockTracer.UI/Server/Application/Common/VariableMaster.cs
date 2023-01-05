using System.Text.RegularExpressions;
using SequentialGuid;

namespace MockTracer.UI.Server.Application.Common;

public static class VariableMaster
{
  private static readonly Regex nameClearRegex = new Regex(string.Format("[{0}]",  Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))));

  public static Guid Next()
  {
    return SequentialGuidGenerator.Instance.NewGuid();
  }

  public static DateTime CurrentTime()
  {
    return DateTime.Now;
  }

  public static string ClearFileName(this string source)
  {
    return nameClearRegex.Replace(source, "_").Replace("=", "_").Replace("&", "_").Replace("-", "_").Replace("__", "_");
  }
}
