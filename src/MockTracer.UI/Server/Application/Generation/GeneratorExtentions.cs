using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Server.Application.Generation.IntenalTest;

namespace MockTracer.UI.Server.Application.Generation;

public static class GeneratorExtentions
{
  public static IServiceCollection RegisterGenerator(this IServiceCollection services)
  {
    services.AddScoped<TestClassGenerator>();
    services.AddScoped<IBuilderResolver, BuilderResolver>();
    
    services.AddScoped<XunitMockTemplateBuilder>();
    services.RegisterByInterface<TracerBuilderBase>().RegisterByInterface<MockBuilderBase>();
    services.AddScoped<InternalTestClassGenerator>();
    services.AddScoped<VariableNameReslover>();
    return services;
  }

  private static IServiceCollection RegisterByInterface<I>(this IServiceCollection services)
    {
    var interaface = typeof(I);
    foreach (var type in interaface.Assembly.GetTypes().Where(w => !w.IsAbstract && interaface.IsAssignableFrom(w)))
    {
      services.AddScoped(type);
    }

    return services;
  }
}
