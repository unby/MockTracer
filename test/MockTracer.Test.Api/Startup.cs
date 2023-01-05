using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MockTracer.Test.Api.Application.Features.Data;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.Database;
using MockTracer.Test.Api.Infrastracture.External;
using MockTracer.UI.Server;
using MockTracer.UI.Server.Application.Watcher;
using Refit;

namespace MockTracer.Test.Api;

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  // This method gets called by the runtime. Use this method to add services to the container.
  public void ConfigureServices(IServiceCollection services)
  {

    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
    });
    services.AddMediatR(Assembly.Load(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

    services.AddScoped<IBlogDbContext, BlogDbContext>();
    services.AddScoped<IDataSource, DataSource>();
    services.AddScoped<IDbProvider, DbProvider>(s => new DbProvider("Filename=Blog.db"));
    services.AddDbContext<BlogDbContext>(options =>
                    options.UseSqlite("Filename=Blog.db"));
    services.AddRefitClient<ICatService>().ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetValue<string>("CatApiUrl")));
    services.UseMockTracerUiService((o, s) => s.DecorateDbProvider<IDbProvider>());
  }

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
    }

    app.UseHttpsRedirection();
    

    app.UseRouting();
    app.UseMockTracerUiApp();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });
  }
}
