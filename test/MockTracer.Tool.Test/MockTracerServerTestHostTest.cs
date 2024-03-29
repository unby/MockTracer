﻿using Ardalis.HttpClientTestExtensions;
using MockTracer.Test;
using MockTracer.Test.Api.Application.Features.Topic;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Generation.Common;
using MockTracer.UI.Server.Application.Watcher;
using Xunit.Abstractions;

namespace MockTracer.Tool.Test;

public class MockTracerServerTestHostTest : ToolTestBase
{
    public MockTracerServerTestHostTest(ITestOutputHelper output)
      : base(output)
    {
    }

    [Fact]
    public async Task Should_check_App_registrationsAsync()
    {
        var host = NewServer();
        var result = await host.GetHttpClient().GetAndDeserializeAsync<IEnumerable<TopicDto>>("/api/topic/v10");

        Assert.Contains(result, i => i.Title == SeedData.TestTopic.Title);
    }

    [Fact]
    public void Should_register_tool()
    {
        Assert.NotNull(NewServer().GetInstance<ScopeWatcher>());
    }

    [Fact]
    public void VariableNameResloverTest()
    {
        const string name = "data";
        var varResolver = new VariableNameReslover();
        Assert.Equal("data", varResolver.CheckName(name));
        Assert.Equal("data2", varResolver.CheckName(name));
        Assert.Equal("data3", varResolver.CheckName(name));
    }

    [Fact]
    public void TitleFormatTest()
    {
        Assert.Equal("Data", "data".ToTitle());
    }


    [Fact]
    public void FindGenericTypeTest()
    {
        Assert.Equal(typeof(IEnumerable<string>), "System.Collections.Generic.IEnumerable`1[System.String]".FindType());
    }

    [Fact]
    public void FindGenericTypeInCodeBracketTest()
    {
        Assert.Equal(typeof(IEnumerable<string>), "System.Collections.Generic.IEnumerable<System.String>".FindType());
    }

    [Fact]
    public void FindGenericTypeWithCustomAssemplyTest()
    {
        Assert.Equal(typeof(IEnumerable<TopicDto>), "System.Collections.Generic.IEnumerable<MockTracer.Test.Api.Application.Features.Topic.TopicDto>".FindType());
    }

    [Fact]
    public void FindSimpleTypeTopicDtoTest()
    {
        Assert.Equal(typeof(TopicDto), "MockTracer.Test.Api.Application.Features.Topic.TopicDto".FindType());
    }

    [Fact]
    public void FindSimpleTypeTest()
    {
        Assert.Equal(typeof(string), "System.String".FindType());
    }
}
