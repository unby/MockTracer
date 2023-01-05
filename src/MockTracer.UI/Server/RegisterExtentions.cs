using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Emit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Presentation;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Server.Application.Watcher;
using MockTracer.UI.Server.Application.Watcher.AspNetMiddleware;
using MockTracer.UI.Server.Application.Watcher.Database;
using MockTracer.UI.Server.Options;

namespace MockTracer.UI.Server;

public static class RegisterExtentions
{
  internal const string ExtentionsState = "MOCKTRACER_ENABLE";
  internal const string Prefix = "mocktracer/";

  internal static bool IsRegister
  {
    get
    {
      var variable = Environment.GetEnvironmentVariable(ExtentionsState);
      if (variable != null && variable.Equals("true", StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      return false;
    }
  }

  internal static bool IsDev
  {
    get
    {
      var variable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      if (variable != null && variable.Equals("Development", StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      return false;
    }
  }

  public static IServiceCollection UseMockTracerUiService(this IServiceCollection services, Action<MockTracerOption, IServiceCollection> replaceAction = null)
  {
    if (IsRegister)
    {
      services.AddMvc(opts =>
      {
        var prefixConvention = new ApiPrefixConvention(Prefix, (c) => c.ControllerType.Namespace.StartsWith("MockTracer.UI.Server"));
        opts.Conventions.Insert(0, prefixConvention);
      });
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatorRequestTrace<,>));
      services.AddDbContext<MockTracerDbContext>(options =>
               options.UseSqlite("Filename=MockTracer.db"));
      services.AddScoped<ScopeWathcer>();
      services.AddOptions<MockTracerOption>();
      replaceAction?.Invoke(new MockTracerOption(), services);
      services.AddScoped<TestClassGenerator>();
      services.AddSingleton(s => s.GetRequiredService<IOptions<MockTracerOption>>().Value.GenerationSetting);

      services.AddHttpContextAccessor();
      services.AddScoped<TraceRepository>();
      services.AddControllers(o => o.Filters.Add<ActionFilterTracer>()).AddJsonOptions(options =>
      {
        // options.JsonSerializerOptions.cy
      });

      services.RegisterGenerator();
      services.AddScoped<HttpClientTraceHandler>();
      services.PostConfigureAll<HttpClientFactoryOptions>(options =>
      {
        options.HttpMessageHandlerBuilderActions.Add(builder =>
        {
          builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<HttpClientTraceHandler>());
        });
      });
    }

    return services;
  }

  public static IApplicationBuilder UseMockTracerUiApp(this IApplicationBuilder app)
  {
    if (IsRegister)
    {
      if (IsDev)
      {
        app.UseDeveloperExceptionPage();
        app.UseWebAssemblyDebugging();
      }


      app.UseBlazorFrameworkFiles();
      app.UseStaticFiles("/" + Prefix.TrimEnd('/'));
      app.UseRouting();
      app.UseMiddleware<HttpContextTrace>();
      app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/mocktracer"), first =>
      {
        first.UseBlazorFrameworkFiles("/mocktracer");
        first.UseStaticFiles();

        first.UseRouting();
        first.UseEndpoints(endpoints =>
        {
          endpoints.MapControllers();
          endpoints.MapFallbackToFile("mocktracer/{*path:nonfile}", "mocktracer/index.html");
        });
      });

      using (var scope = app.ApplicationServices.CreateScope())
      {
        var dbContext = scope.ServiceProvider.GetRequiredService<MockTracerDbContext>();
        dbContext.Database.EnsureCreated();
      }
    }

    return app;
  }

  public static IServiceCollection DecorateDbProvider<T>(this IServiceCollection services)
  {
    var t = GenerateDbProvider<T>();
    services.Decorate(typeof(T), t);
    return services;
  }


  /// <summary>
  /// Create type with logic decorate of dbConnection provider
  /// </summary>
  /// <typeparam name="T">interface { DbConnection Get() ;} </typeparam>
  /// <returns>class extented T</returns>
  /// <exception cref="ArgumentException"></exception>
  public static Type GenerateDbProvider<T>()
  {
    var @interface = typeof(T);
    if (!@interface.IsInterface)
    {
      throw new ArgumentException("T type must be interface");
    }

    var methods = @interface.GetMethods().Where(w => w.GetParameters().Length == 0
        && (w.ReturnType == typeof(IDbConnection) || w.ReturnType == typeof(DbConnection))).ToArray();

    if (!methods.Any() || methods.Length != @interface.GetMethods().Length)
    {
      throw new ArgumentException("T type must have only the methods without arguments and out type is IDbConnection (or DbConnection)");
    }

    AssemblyName assemblyName = new AssemblyName("MockTracker.Test.Runtime");
    AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#pragma warning disable CS8604 // Possible null reference argument.
    ModuleBuilder mb = ab.DefineDynamicModule(assemblyName.Name);
#pragma warning restore CS8604 // Possible null reference argument.

    TypeBuilder tb = mb.DefineType(
        @interface.Name + "Trace",
         TypeAttributes.Public);
    tb.AddInterfaceImplementation(@interface);

    FieldBuilder dbProviderField = tb.DefineField(
        "_dbProvider",
        @interface,
        FieldAttributes.Private);

    var scopeWathcerType = typeof(ScopeWathcer);
    FieldBuilder scopeWathcerField = tb.DefineField(
        "_scopeWathcer",
        scopeWathcerType,
        FieldAttributes.Private);

    var getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static); // Type.GetTypeFromHandle(); 

    ConstructorBuilder ctor1 = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { @interface, scopeWathcerType });
    ILGenerator ctor1IL = ctor1.GetILGenerator();
    ctor1IL.Emit(OpCodes.Ldarg_0);
#pragma warning disable CS8604 // Possible null reference argument.
    ctor1IL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
#pragma warning restore CS8604 // Possible null reference argument.
    ctor1IL.Emit(OpCodes.Ldarg_0);
    ctor1IL.Emit(OpCodes.Ldarg_1);
    ctor1IL.Emit(OpCodes.Stfld, dbProviderField);
    ctor1IL.Emit(OpCodes.Ldarg_0);
    ctor1IL.Emit(OpCodes.Ldarg_2);
    ctor1IL.Emit(OpCodes.Stfld, scopeWathcerField);
    ctor1IL.Emit(OpCodes.Ret);

    ConstructorInfo ctor = typeof(DBConnectionTracer).GetConstructors()[0];
    foreach (var methodInfo in methods)
    {
      MethodBuilder method = tb.DefineMethod(
            @interface.Name + "." + methodInfo.Name,
            MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
            methodInfo.ReturnType,
            Type.EmptyTypes);

      ILGenerator methIL = method.GetILGenerator();
      methIL.Emit(OpCodes.Ldarg_0);
      methIL.Emit(OpCodes.Ldfld, dbProviderField);
      methIL.Emit(OpCodes.Callvirt, methodInfo);
      methIL.Emit(OpCodes.Ldarg_0);
      methIL.Emit(OpCodes.Ldfld, scopeWathcerField);
      methIL.Emit(OpCodes.Ldtoken, @interface);
#pragma warning disable CS8604 // Possible null reference argument.
      methIL.Emit(OpCodes.Call, getTypeFromHandleMethod);
#pragma warning restore CS8604 // Possible null reference argument.
      methIL.Emit(OpCodes.Newobj, ctor);
      methIL.Emit(OpCodes.Ret);
      tb.DefineMethodOverride(method, methodInfo);
    }

    var cash = tb.CreateType();

    return cash ?? throw new NullReferenceException($"Type extedted {@interface.Name} didn.t create");
  }
}
