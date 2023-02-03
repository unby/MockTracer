using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;

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
    var original = result;
    if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task))
    {
      return null;
    }

    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
    {
      original = type.GetProperty("Result").GetValue(result);
      type = type.GenericTypeArguments[0];
    }

    return new ArgumentObjectInfo()
    {
      ArgumentName = name,
      OriginalObject = original,
      ClassName = type.GetRealTypeName(),
      Namespace = type.Namespace,
    };
  }
}
