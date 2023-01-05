using JustEat.HttpClientInterception;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MockTracer.Test;

public abstract class TestBase<TEntryPoint> where TEntryPoint : class
{
  /// <summary>
  ///     Initializes a new instance of the <see cref="TestBase" /> class.
  /// </summary>
  /// <param name="output">тест лог</param>
  public TestBase(ITestOutputHelper output)
  {
    Log = output;
    HttpClientInterceptor = new HttpClientInterceptorOptions()
      .ThrowsOnMissingRegistration();
    HttpClientInterceptor.OnSend = (request) =>
    {
      Log.WriteLine($"HTTP {request.Method} {request.RequestUri}");
      return Task.CompletedTask;
    };
  }

  /// <summary>
  /// Mock http services
  /// </summary>
  protected HttpClientInterceptorOptions HttpClientInterceptor { get; }

  /// <summary>
  /// Console logging
  /// </summary>
  protected ITestOutputHelper Log { get; }

  protected virtual string[] ApplicationArguments => new string[0];

  /// <summary>
  /// Build app instance
  /// </summary>
  /// <param name="overrideCollection">функция внедрения Mock объектов</param>
  /// <returns>TariffService</returns>
  protected virtual IHost NewServer(
      Action<IServiceCollection>? overrideCollection = null,
      [System.Runtime.CompilerServices.CallerMemberName] string testName = "")
  {
    var hostBuilder = ConfigureBuilder()
        .ConfigureServices(s =>
        {
          s.AddSingleton<IHttpMessageHandlerBuilderFilter, InterceptionFilter>(
                            (_) => new InterceptionFilter(HttpClientInterceptor));
          s.AddLogging((builder) => builder.AddXUnit(Log));
        })
        .ConfigureServices(s => ConfigureHost(testName)(s))
        .ConfigureServices(overrideCollection == null ? (c) => { }
    : overrideCollection);

    var testServer = hostBuilder.Build();

    AfterBuildConfiguration(testServer);

    testServer.Start();

    return testServer;
  }

  protected virtual void AfterBuildConfiguration(IHost testServer)
  {
  }

  protected abstract Action<IServiceCollection> ConfigureHost(string testName);

  protected virtual IHostBuilder CreateBuilderbHost()
  {
    return Host.CreateDefaultBuilder(ApplicationArguments).ConfigureWebHostDefaults(webBuilder =>
      {
        webBuilder.UseStartup<TEntryPoint>().UseTestServer();
      });
  }


  protected virtual IHostBuilder ConfigureBuilder()
  {
    var deferredHostBuilder = new DeferredHostBuilder(ApplicationArguments);
    deferredHostBuilder.UseEnvironment(Environments.Development);
    // There's no helper for UseApplicationName, but we need to 
    // set the application name to the target entry point 
    // assembly name.
    deferredHostBuilder.ConfigureHostConfiguration(config =>
    {
      config.AddInMemoryCollection(new Dictionary<string, string?>
              {
                        { HostDefaults.ApplicationKey, typeof(TEntryPoint).Assembly.GetName()?.Name ?? string.Empty }
              });
    });
    // This helper call does the hard work to determine if we can fallback to diagnostic source events to get the host instance
    var factory = HostFactoryResolver.ResolveHostFactory(
        typeof(TEntryPoint).Assembly,
        stopApplication: false,
        configureHostBuilder: deferredHostBuilder.ConfigureHostBuilder,
        entrypointCompleted: deferredHostBuilder.EntryPointCompleted);

    if (factory is not null)
    {
      // If we have a valid factory it means the specified entry point's assembly can potentially resolve the IHost
      // so we set the factory on the DeferredHostBuilder so we can invoke it on the call to IHostBuilder.Build.
      deferredHostBuilder.SetHostFactory(factory);
      deferredHostBuilder.ConfigureWebHostDefaults(webBuilder =>
      {
        webBuilder.ConfigureTestServices(s => s.AddControllers().AddApplicationPart(typeof(TEntryPoint).Assembly).AddControllersAsServices());
        webBuilder.UseTestServer();
        webBuilder.UseDefaultServiceProvider(a => a.ValidateScopes = false);

      });

      return deferredHostBuilder;
    }
    else
    {
      return CreateBuilderbHost();
    }
  }


}
