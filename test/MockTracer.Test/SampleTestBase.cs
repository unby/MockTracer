using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MockTracer.Test.Api.Controllers;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.Database;
using Xunit.Abstractions;

namespace MockTracer.Test;

public class SampleTestBase : TestBase<TopicController>
{
    private string[] TestArgs = new string[] { "CatApiUrl=https://test.api" };
    public SampleTestBase(ITestOutputHelper output)
      : base(output)
    {
    }

    protected override string[] ApplicationArguments => base.ApplicationArguments.Union(TestArgs).ToArray();

    protected override Action<IServiceCollection> ConfigureHost(string testName)
    {
        return (s) =>
        {
            s.RemoveAll<DbContextOptions<BlogDbContext>>();

            s.AddDbContext<BlogDbContext>(options =>
                        options.UseInMemoryDatabase("BlogDbContext" + testName));
            s.AddScoped<IBlogDbContext, BlogDbContext>();
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
                context.Database.EnsureCreated();
                SeedData.AddDataToContext(context);
            }
            catch (Exception ex)
            {
                Log.WriteLine("An error occurred seeding the DB." + ex.ToString());
            }
        }
    }
}
