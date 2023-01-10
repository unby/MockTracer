using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.Tool.Test.AutoTest;

public class HttpContext_test : ToolTestBase
{
  public HttpContext_test(Xunit.Abstractions.ITestOutputHelper output)
    : base(output)
  {
  }

  [Fact]
  public async Task Should_create_HttpContext_template_Async()
  {
    var generator = NewServer().Services.GetRequiredService<TestClassGenerator>();
    var newClass = await generator.CreateAsync(Attributes);

    Assert.NotNull(newClass.SourceCode);
    Log.WriteLine(newClass.SourceCode);
  }

  protected GenerationAttributes Attributes => new GenerationAttributes
  {
    ScopeId = new Guid("08dad941-ad87-2581-c87e-ba36e4034578"),
    InputId = new Guid("08dad941-ad88-956f-c87e-ba36e4034579"),
    OutputId = new Guid[]
  {
    new Guid("08dad941-b022-37af-c87e-ba36e403457d")
  },
    TestName = "Test.NameSpace.FirstTestClass.testmethod"
  };

  protected override void AfterBuildConfiguration(IHost host)
  {
    base.AfterBuildConfiguration(host);
    using var scope = host.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<MockTracerDbContext>();
    context.AddRange(new object[]
  {
  new StackRow
  {
    Id = new Guid("08dad941-ad88-956f-c87e-ba36e4034579"),
    IsEntry = true,
    Time = DateTime.ParseExact("2022-12-08T22:28:53.3869184", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
    DeepLevel = 1,
    StackTrace = "   at MockTracer.UI.Server.Application.Watcher.AspNetMiddleware.HttpContextTrace.MakeInfo(String title)\r\n   at MockTracer.UI.Server.Application.Watcher.AspNetMiddleware.HttpContextTrace.Invoke(HttpContext context, ScopeWathcer _scopeStore)\r\n   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)\r\n   at MockTracer.UI.Server.Application.Watcher.AspNetMiddleware.HttpContextTrace.Invoke(HttpContext context, ScopeWathcer _scopeStore)\r\n   at Microsoft.AspNetCore.Builder.UseMiddlewareExtensions.<>c__DisplayClass5_1.<UseMiddleware>b__2(HttpContext context)\r\n   at Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware.Invoke(HttpContext httpContext)\r\n   at Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.Builder.Extensions.MapWhenMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.Builder.Extensions.MapMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)\r\n   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)\r\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware.Invoke(HttpContext httpContext)\r\n   at Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware.Invoke(HttpContext context)\r\n   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)\r\n   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)\r\n   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)\r\n   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)\r\n   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)\r\n   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)\r\n   at Microsoft.AspNetCore.Builder.UseMiddlewareExtensions.<>c__DisplayClass5_1.<UseMiddleware>b__2(HttpContext context)\r\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)\r\n   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)\r\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.HostFiltering.HostFilteringMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.WebTools.BrowserLink.Net.BrowserLinkMiddleware.InvokeAsync(HttpContext context)\r\n   at Microsoft.WebTools.BrowserLink.Net.VsContentMiddleware.InvokeAsync(HttpContext context)\r\n   at Microsoft.AspNetCore.Watch.BrowserRefresh.BrowserRefreshMiddleware.InvokeAsync(HttpContext context)\r\n   at System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start[TStateMachine](TStateMachine& stateMachine)\r\n   at Microsoft.AspNetCore.Watch.BrowserRefresh.BrowserRefreshMiddleware.InvokeAsync(HttpContext context)\r\n   at Microsoft.AspNetCore.Builder.Extensions.MapWhenMiddleware.Invoke(HttpContext context)\r\n   at Microsoft.AspNetCore.Hosting.HostingApplication.ProcessRequestAsync(Context context)\r\n   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.ProcessRequests[TContext](IHttpApplication`1 application)\r\n   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)\r\n   at System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AsyncStateMachineBox`1.MoveNext(Thread threadPoolThread)\r\n   at System.Threading.ThreadPoolWorkQueue.Dispatch()\r\n   at System.Threading.PortableThreadPool.WorkerThread.WorkerThreadStart()\r\n   at System.Threading.Thread.StartCallback()\r\n",
    Title = "/api/topic/v10?",
    TracerType = "HttpContext",
    ScopeId = new Guid("08dad941-ad87-2581-c87e-ba36e4034578"),
    Order = 1
  },
  new Output
  {
    Id = new Guid("08dad941-b03d-e4a4-c87e-ba36e4034581"),
    Name = "response",
    ClassName = "DefaultHttpResponse",
    Namespace = "Microsoft.AspNetCore.Http",
    Json = "{\r\n  \"StatusCode\": 200,\r\n  \"Body\": \"7\",\r\n  \"Object\": null,\r\n  \"ContentType\": \"application/json; charset=utf-8\"\r\n}",
    SharpCode = "new TraceHttpReponse\r\n{\r\n  StatusCode = 200,\r\n  Body = \"7\",\r\n  ContentType = \"application/json; charset=utf-8\"\r\n}",
    StackRowId = new Guid("08dad941-ad88-956f-c87e-ba36e4034579"),
    ShortView = string.Empty

  },
  new Input
  {
    Id = new Guid("08dad941-ad95-dac2-c87e-ba36e403457a"),
    Name = "request",
    ClassName = "DefaultHttpRequest",
    Namespace = "Microsoft.AspNetCore.Http",
    Json = "{\r\n  \"Body\": \"{\\\"title\\\":\\\"strin sdfsd sdf sdg\\\",\\\"content\\\":\\\"string\\\",\\\"authorId\\\":2}\",\r\n  \"Object\": null,\r\n  \"Path\": \"/api/topic/v10\",\r\n  \"FullPath\": \"/api/topic/v10?\",\r\n  \"ContentType\": \"application/json\",\r\n  \"Method\": \"POST\"\r\n}",
    SharpCode = "new TraceHttpRequest\r\n{\r\n  Body = \"{\\\"title\\\":\\\"strin sdfsd sdf sdg\\\",\\\"content\\\":\\\"string\\\",\\\"authorId\\\":2}\",\r\n  Path = \"/api/topic/v10\",\r\n  FullPath = \"/api/topic/v10?\",\r\n  ContentType = \"application/json\",\r\n  Method = \"POST\"\r\n}",
    StackRowId = new Guid("08dad941-ad88-956f-c87e-ba36e4034579"),
    ShortView = string.Empty
  }
  });
    context.SaveChanges();
  }
}
