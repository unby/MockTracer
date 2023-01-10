using MockTracer.Test.Api.Infrastracture.Database;
/*
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.Services.AddControllersWithViews();
services.AddControllers();
builder.Services.AddRazorPages();

services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
});
services.AddMediatR(Assembly.Load(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

services.AddScoped<IBlogDbContext, BlogDbContext>();
services.AddDbContext<BlogDbContext>(options =>
                options.UseSqlite("Filename=Blog.db"));
services.AddRefitClient<ICatService>().ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("CatApiUrl")));
services.AddScoped<IDataSource, DataSource>();
services.AddScoped<IDbProvider, DbProvider>(s => new DbProvider("Filename=Blog.db"));

services.UseMockTracerUiService((o, s) => s.DecorateDbProvider<IDbProvider>());
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(endpoints => {
  endpoints.MapControllers();
  endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}"
  );
});
app.UseMockTracerUiApp();
// Seed Database
using (var scope = app.Services.CreateScope())
{
  var configureContexxt = scope.ServiceProvider;

  try
  {
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    dbContext.Database.EnsureCreated();
  }
  catch (Exception ex)
  {
    var logger = configureContexxt.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
  }
}

app.Run();

*/
namespace MockTracer.Test.Api;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var builder = CreateHostBuilder(args).Build();
            using (var scope = builder.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
                dbContext.Database.EnsureCreated();
            }
            builder.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 400;
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

