using Ardalis.HttpClientTestExtensions;
using MockTracer.Test.Api.Application.Features.Topic;
using MockTracer.UI.Server.Application.Watcher;
using Xunit.Abstractions;

namespace MockTracer.Test;

public class TestHostAppTest : SampleTestBase
{
    public TestHostAppTest(ITestOutputHelper output)
      : base(output)
    {
    }

    [Fact]
    public async Task Should_check_App_registrationsAsync()
    {
        var result = await NewServer().GetHttpClient().GetAndDeserializeAsync<IEnumerable<TopicDto>>("/api/topic/v10");

        Assert.NotEmpty(result);
        Assert.Contains(result, i => i.Title == SeedData.TestTopic.Title);
    }

    [Fact]
    public async Task Should_not_register_toolAsync()
    {
        await Assert.ThrowsAsync<Exception>(() => Task.FromResult(NewServer().GetInstance<ScopeWatcher>()));
    }
}
