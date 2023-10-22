using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MockTracer.Test.Api.Application.Features.SQL;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.Database;
using MockTracer.Test.Api.Infrastracture.External;
using Refit;

namespace MockTracer.Test.Api;

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  public void ConfigureServices(IServiceCollection services)
  {

    services.AddControllers();
    services.AddProblemDetails();
    services.AddSwaggerGen(c =>
    {
      c.EnableAnnotations();
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
    });
    services.AddMediatR(Assembly.Load(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

    services.AddScoped<IBlogDbContext, BlogDbContext>();
    services.AddScoped<IDataSource, DataSource>();
    services.AddScoped<IDbProvider, DbProvider>(s => new DbProvider("Filename=Blog.db"));
    services.AddDbContext<BlogDbContext>(options =>
                    options.AddInterceptors(new KeyOrderingExpressionInterceptor()).UseSqlite("Filename=Blog.db").UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));
    services.AddRefitClient<ICatService>().ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetValue<string>("CatApiUrl")));

    services.UseMockTracerUiService((s) =>
    {
      s.DecorateDbProvider<IDbProvider>();
      s.DecorateVirtual<IDataSource>();
    },
      s => { s.GenerationSetting.DefaultFolder = @"..\MockTracer.Test\Generated"; s.GenerationSetting.FileExtentions = "cs"; } );
    Console.WriteLine(Environment.CurrentDirectory);
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
    }

    app.UseHttpsRedirection();

    app.UseProblemDetails();
    app.UseRouting();

    app.UseMockTracerUiApp();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });
  }
}
