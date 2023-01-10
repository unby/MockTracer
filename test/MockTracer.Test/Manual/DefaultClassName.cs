using MediatR;
using MockTracer.Test.Api.Application.Features.Topic;
using Moq;
using System.Net;
using Xunit.Abstractions;

namespace MockTracer.Test.Manual;
public class DefaultClassName : SampleTestBase
{
  public DefaultClassName(ITestOutputHelper output) : base(output)
  {
  }

  [Fact]
  public async Task DefaultMethodNameAsync()
  {
    // prepare
    var mockrequestObject = new Mock<IRequestHandler<CreateTopicCommand, int>>();
    mockrequestObject.Setup(s => s.Handle(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(3));
    // action services => { }
    var host = NewServer((s) => s.Replace(mockrequestObject));
    var httpResult = await host.GetHttpClient().PostAsync("/api/topic/v10?", new CreateTopicCommand { Title = "string", Content = "string", AuthorId = 2 }.ToHttpContent());
    // todo: assert
    Assert.Equal(HttpStatusCode.OK, httpResult.StatusCode);
    Assert.Equal("3", httpResult.Content.ReadAsStringAsync().Result);
  }
}

