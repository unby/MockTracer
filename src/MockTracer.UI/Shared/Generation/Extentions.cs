using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Shared.Generation;

/// <summary>
/// Generation helpers
/// </summary>
public static class Extentions
{
  /// <summary>
  /// make class name
  /// </summary>
  /// <param name="stackRow"></param>
  /// <returns></returns>
  public static string GetClassName(this StackRow stackRow)
  {
    switch (stackRow.TracerType)
    {
      case Constants.Mediatr:
      case Constants.MvcActionFilter:
      case Constants.Custom:
        return (stackRow.DeclaringTypeName ?? stackRow.TracerType) + "Test";
      default:
        return stackRow.TracerType + "Test";
    }
  }

  /// <summary>
  /// make test method
  /// </summary>
  /// <param name="stackRow"></param>
  /// <returns></returns>
  public static string GetMethodName(this StackRow stackRow)
  {
    switch (stackRow.TracerType)
    {
      case Constants.Mediatr:
      case Constants.MvcActionFilter:
      case Constants.Custom:
        return "ItShouldInvoke" + (stackRow.MethodName ?? stackRow.TracerType + "Method");
      default:
        return "ItShouldInvoke" + stackRow.TracerType + "Method";
    }
  }
}
