using System.Data;
using System.Data.Common;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.Emit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
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

      Console.WriteLine("MockerTrace is disabled");
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
      services.AddControllers(o => o.Filters.Add<ActionFilterTracer>()).AddJsonOptions(options => { });

      services.RegisterGenerator();
      services.AddScoped<HttpClientTraceHandler>();
      services.PostConfigureAll<HttpClientFactoryOptions>(options =>
      {
        options.HttpMessageHandlerBuilderActions.Add(builder =>
        {
          builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<HttpClientTraceHandler>());
        });
      });

      Console.WriteLine("MockerTrace services are configured");
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

      app.UseMiddleware<HttpContextTrace>();

#if DEBUG
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
#else
           app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/mocktracer"), first =>
      {
        var options = CreateStaticFilesOptions(new ResourceProvider());
        first.UseEmbededLocalBlazorApp("/mocktracer", options);
        first.UseStaticFiles();
        first.UseRouting();
        first.UseEndpoints(endpoints =>
        {
          endpoints.MapControllers();
          endpoints.MapFallback(HandleFallbackAsync);
          //endpoints.MapFallbackToFile("mocktracer/{*path}", "mocktracer/index.html", options);
        });
      });
#endif



      using (var scope = app.ApplicationServices.CreateScope())
      {
        var dbContext = scope.ServiceProvider.GetRequiredService<MockTracerDbContext>();
        dbContext.Database.EnsureCreated();
      }

      Console.WriteLine("App's MockerTrace is configured");
    }

    return app;
  }

  public static async Task HandleFallbackAsync(HttpContext context)
  {
    var apiPathSegment = new PathString("/mocktracer/data"); // Find out from the request URL if this is a request to the API or just a web page on the Blazor WASM app.
    bool isApiRequest = context.Request.Path.StartsWithSegments(apiPathSegment);

    if (!isApiRequest)
    {
      context.Response.SendFileAsync(MemoryFileInfo.Index); // This is a request for a web page so just do the normal out-of-the-box behaviour.
    }
    else
    {
      context.Response.StatusCode = StatusCodes.Status404NotFound; // This request had nothing to do with the Blazor app. This is just an API call that went wrong.
    }
  }

  /// <summary>
  /// Configures the application to serve Blazor WebAssembly framework files from the path <paramref name="pathPrefix"/>. This path must correspond to a referenced Blazor WebAssembly application project.
  /// </summary>
  /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
  /// <param name="pathPrefix">The <see cref="PathString"/> that indicates the prefix for the Blazor WebAssembly application.</param>
  /// <returns>The <see cref="IApplicationBuilder"/></returns>
  public static IApplicationBuilder UseEmbededLocalBlazorApp(this IApplicationBuilder builder, PathString pathPrefix, StaticFileOptions options)
  {
    if (builder is null)
    {
      throw new ArgumentNullException(nameof(builder));
    }

    var webHostEnvironment = builder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

    builder.MapWhen(ctx =>
    {
      return ctx.Request.Path.StartsWithSegments(pathPrefix, out var rest) && Path.HasExtension(rest) && !rest.StartsWithSegments("/data");
    },
    subBuilder =>
    {
      subBuilder.Use(async (context, next) =>
      {
        context.Response.Headers.Append("Blazor-Environment", webHostEnvironment.EnvironmentName);

        if (webHostEnvironment.IsDevelopment())
        {
          // DOTNET_MODIFIABLE_ASSEMBLIES is used by the runtime to initialize hot-reload specific environment variables and is configured
          // by the launching process (dotnet-watch / Visual Studio).
          // In Development, we'll transmit the environment variable to WebAssembly as a HTTP header. The bootstrapping code will read the header
          // and configure it as env variable for the wasm app.
          if (Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES") is string dotnetModifiableAssemblies)
          {
            context.Response.Headers.Append("DOTNET-MODIFIABLE-ASSEMBLIES", dotnetModifiableAssemblies);
          }

          // See https://github.com/dotnet/aspnetcore/issues/37357#issuecomment-941237000
          // Translate the _ASPNETCORE_BROWSER_TOOLS environment configured by the browser tools agent in to a HTTP response header.
          if (Environment.GetEnvironmentVariable("__ASPNETCORE_BROWSER_TOOLS") is string blazorWasmHotReload)
          {
            context.Response.Headers.Append("ASPNETCORE-BROWSER-TOOLS", blazorWasmHotReload);
          }
        }

        await next(context);
      });

      // subBuilder.UseMiddleware<ContentEncodingNegotiator>();

      subBuilder.UseStaticFiles(options);
    });

    return builder;
  }

  private static StaticFileOptions CreateStaticFilesOptions(IFileProvider webRootFileProvider)
  {
    var options = new StaticFileOptions();
    options.FileProvider = webRootFileProvider;
    var contentTypeProvider = new FileExtensionContentTypeProvider();
    AddMapping(contentTypeProvider, ".dll", MediaTypeNames.Application.Octet);
    // We unconditionally map pdbs as there will be no pdbs in the output folder for
    // release builds unless BlazorEnableDebugging is explicitly set to true.
    AddMapping(contentTypeProvider, ".pdb", MediaTypeNames.Application.Octet);
    AddMapping(contentTypeProvider, ".br", MediaTypeNames.Application.Octet);
    AddMapping(contentTypeProvider, ".dat", MediaTypeNames.Application.Octet);
    AddMapping(contentTypeProvider, ".blat", MediaTypeNames.Application.Octet);

    options.ContentTypeProvider = contentTypeProvider;

    // Static files middleware will try to use application/x-gzip as the content
    // type when serving a file with a gz extension. We need to correct that before
    // sending the file.
    options.OnPrepareResponse = fileContext =>
    {
      // At this point we mapped something from the /_framework
      fileContext.Context.Response.Headers.Append(HeaderNames.CacheControl, "no-cache");

      var requestPath = fileContext.Context.Request.Path;
      var fileExtension = Path.GetExtension(requestPath.Value);
      if (string.Equals(fileExtension, ".gz") || string.Equals(fileExtension, ".br"))
      {
        // When we are serving framework files (under _framework/ we perform content negotiation
        // on the accept encoding and replace the path with <<original>>.gz|br if we can serve gzip or brotli content
        // respectively.
        // Here we simply calculate the original content type by removing the extension and apply it
        // again.
        // When we revisit this, we should consider calculating the original content type and storing it
        // in the request along with the original target path so that we don't have to calculate it here.
        var originalPath = Path.GetFileNameWithoutExtension(requestPath.Value);
        if (originalPath != null && contentTypeProvider.TryGetContentType(originalPath, out var originalContentType))
        {
          fileContext.Context.Response.ContentType = originalContentType;
        }
      }
    };

    return options;
  }

  private static void AddMapping(FileExtensionContentTypeProvider provider, string name, string mimeType)
  {
    if (!provider.Mappings.ContainsKey(name))
    {
      provider.Mappings.Add(name, mimeType);
    }
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
