using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MockTracer.Test;
using MockTracer.Test.Api.Controllers;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.Database;
using MockTracer.UI.Server.Application.Storage;
using Xunit.Abstractions;

namespace MockTracer.Tool.Test;

public class ToolTestBase : TestBase<TopicController>
{
  private string[] TestArgs = new string[]
  {
    "CatApiUrl=https://test.api",
    "MOCKTRACER_ENABLE=true"
  };

  static ToolTestBase()
  {
    Environment.SetEnvironmentVariable("MOCKTRACER_ENABLE", "true");
  }

  public ToolTestBase(ITestOutputHelper output)
    : base(output)
  {
  }

  protected override string[] ApplicationArguments => base.ApplicationArguments.Union(TestArgs).ToArray();

  protected override Action<IServiceCollection> ConfigureHost(string testName)
  {
    return (s) =>
    {
      s.RemoveAll<DbContextOptions<BlogDbContext>>();

      s.AddDbContext<BlogDbContext>(options => {
        options.UseInMemoryDatabase(nameof(BlogDbContext) + testName + this.GetHashCode());
        options.EnableSensitiveDataLogging();
                      });
      s.AddScoped<IBlogDbContext, BlogDbContext>();

      s.RemoveAll<DbContextOptions<MockTracerDbContext>>();
      s.AddDbContext<MockTracerDbContext>(options =>
                     options.UseInMemoryDatabase(nameof(MockTracerDbContext) + testName + this.GetHashCode()));
    };
  }

  protected override void AfterBuildConfiguration(IHost host)
  {
    using (var scope = host.Services.CreateScope())
    {
      var services = scope.ServiceProvider;
      try
      {
        using var context = services.GetRequiredService<BlogDbContext>();
        
        SeedData.AddDataToContext(context);
        context.SaveChanges();
      }
      catch (Exception ex)
      {
        Log.WriteLine("An error occurred seeding the DB." + ex.ToString());
        throw;
      }
    }
  }
}
