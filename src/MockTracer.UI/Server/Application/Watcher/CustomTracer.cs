using MockTracer.UI.Server.Application.Common;

namespace MockTracer.UI.Server.Application.Watcher;

public static class CustomTracer
{
  public static TraceInfo CreateInfo(string title)
  {
    return new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.Custom,
    };
  }

  public static ArgumentObjectInfo ResolveArgument(object result, Type type, string name)
  {
    return new ArgumentObjectInfo()
    {
      ArgumentName = name,
      OriginalObject = result,
      ClassName = type.Name,
      Namespace = type.Namespace,
    };
  }
}
