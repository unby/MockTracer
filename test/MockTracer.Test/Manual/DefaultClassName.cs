using MediatR;
using MockTracer.Test;
using MockTracer.Test.Api.Application.Features.Topic;
using Moq;
using System;
using System.Net;
using Xunit.Abstractions;

namespace DefaultNameSpace;
public class DefaultClassName : SampleTestBase
{
  public DefaultClassName(ITestOutputHelper output) : base(output)
  {
  }

  [Fact]
  public async Task DefaultMethodNameAsync()
  {
    // prepare
    var mockrequestObject = new Mock<IRequestHandler<CreateTopicCommand, Int32>>();
    mockrequestObject.Setup(s => s.Handle(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<Int32>(3));
    // action services => { }
    var host = NewServer((s) => s.Replace(mockrequestObject));
    var httpResult = await host.GetHttpClient().PostAsync("/api/topic/v10?", new CreateTopicCommand { Title = "string", Content = "string", AuthorId = 2 }.ToHttpContent());
    // todo: assert
    Assert.Equal(HttpStatusCode.OK, httpResult.StatusCode);
    Assert.Equal("3", httpResult.Content.ReadAsStringAsync().Result);
  }
}

