using System.Text;
using MockTracer.UI.Server.Application.Presentation;
using MockTracer.UI.Server.Options;

namespace MockTracer.UI.Server.Application.Generation;

/// <summary>
/// TemplateBuilderBase
/// </summary>
public abstract class TemplateBuilderBase : ITemplateBuilder
{
  /// <summary>
  /// Template variable list
  /// Order is important
  /// </summary>
  private static string[] ReplaceSpaces = new string[]
  {
    "{{using}}", "{{nameSpace}}", "{{nameSpace[}}", "{{nameSpace]}}",
    "{{className}}", "{{methodName}}", "{{TestBase}}",
    "{{Prepare}}", "{{Action}}", "{{Assert}}", "{{BigVariable}}", "(s) => {{Configure}}", "{{Configure}}" 
  };

  /// <summary>
  /// Build units factory 
  /// </summary>
  protected readonly IBuilderResolver _builderResolver;

  /// <summary>
  /// TemplateBuilderBase
  /// </summary>
  /// <param name="builderResolver"><see cref="IBuilderResolver"/></param>
  protected TemplateBuilderBase(IBuilderResolver builderResolver)
  {
    _builderResolver = builderResolver;
  }

  /// <summary>
  /// Template string
  /// </summary>
  protected abstract string Template { get; }

  /// <summary>
  /// Concat test
  /// </summary>
  /// <param name="params"></param>
  /// <param name="settings"></param>
  /// <param name="tracerBuilder"></param>
  /// <param name="mockerBuilders"></param>
  /// <returns></returns>
  public virtual string Build(Shared.Generation.GenerationAttributes @params, ClassGenerationSetting settings, GenerationContext context)
  {
    StringBuilder builder = NameReplace(settings);
    var data = new List<LineFragment>(_builderResolver.ResolveInputBuilder(context.Input.TracerType).BuildFragments(context.Input));
    foreach (var item in context.Output)
    {
      var mocker = _builderResolver.ResolveMockBuilder(item.TracerType);
      data.AddRange(mocker.BuildFragments(item));
    }

    foreach (var item in data.GroupBy(g => g.Code))
    {
      if (item.Key == BuildingConstans.Configure)
      {
        if (item.Count() > 1)
        {
          builder.Replace(item.Key, $"{Environment.NewLine} {{ {string.Join(Environment.NewLine, item.Select(s => s.Line + ";").Distinct())} {Environment.NewLine} }}");
        }
        else
        {
          builder.Replace(item.Key, item.FirstOrDefault().Line);
        }
      }
      else
      {
        builder.Replace(item.Key, string.Join(Environment.NewLine, item.Select(s=>s.Line).Distinct()));
      }

    }

    Clear(builder);
    return builder.ToString().ArrangeUsingRoslyn();
  }

  /// <summary>
  /// remove label
  /// </summary>
  /// <param name="builder"></param>
  private void Clear(StringBuilder builder)
  {
    foreach (var item in ReplaceSpaces)
    {
      builder.Replace(item, string.Empty);
    }
  }

  /// <summary>
  /// Build class info
  /// </summary>
  /// <param name="settings"></param>
  /// <returns></returns>
  protected StringBuilder NameReplace(ClassGenerationSetting settings)
  {
    var sb = new StringBuilder(Template);
    sb = sb.Replace("{{className}}", settings.DefaultClassName)
       .Replace("{{methodName}}", settings.DefaultMethodName)
        .Replace("{{TestBase}}", settings.TestBase)
       .Replace("{{nameSpace}}", settings.IsWriteNameSpaceBracket ? settings.DefaultNameSpace : settings.DefaultNameSpace + ";");

    if (settings.IsWriteNameSpaceBracket)
    {
      sb = sb.Replace("{{nameSpace[}}", Environment.NewLine + "{")
       .Replace("{nameSpace]}}", Environment.NewLine + "}");
    }

    return sb;
  }
}
