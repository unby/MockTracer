using System.Net;
using System.Text;
using JustEat.HttpClientInterception;
using MediatR;
using MockTracer.Test.Api.Application.Features.Data;
using MockTracer.Test.Api.Application.Features.Topic;
using MockTracer.Test.Api.Controllers;
using MockTracer.Test.Api.Domain;
using Moq;
using Xunit.Abstractions;

namespace MockTracer.Test.Manual;

public class HttpClientTest : SampleTestBase
{
  public HttpClientTest(ITestOutputHelper output) : base(output)
  {
  }

  [Fact]
  public async Task HttpClientMockTestAsync()
  {
    // prepare
    var builder = new HttpRequestInterceptionBuilder().Requests().For(f => f.Method == HttpMethod.Get && f.RequestUri.AbsolutePath.EndsWith("fact"))
               .Responds().WithMediaType("application/json")
               .WithContent(() => Encoding.UTF8.GetBytes(@"{
  ""fact"": ""The domestic cat is the only species able to hold its tail vertically while walking. You can also learn about your cat's present state of mind by observing the posture of his tail."",
  ""length"": 180
}")).WithStatus(HttpStatusCode.OK)
               .RegisterWith(HttpClientInterceptor);

    // act
    ApplicationArguments.Equals(ApplicationArguments);
    var host = NewServer(services => { });
    var result = await host.GetHttpClient().GetAsync("/api/topic/v10/fact");

    // :todo add additional assertions
    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
  }

  [Fact]
  public async Task MediatorMockTestAsync()
  {
    // prepare
    var mockObject = new Mock<IRequestHandler<CatFactQuery, CatFact>>();
    mockObject.Setup(s => s.Handle(It.IsAny<CatFactQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<CatFact>(null));

    // act
    ApplicationArguments.Equals(ApplicationArguments);
    var result = await NewServer(s => s.Replace(mockObject)).GetHttpClient().GetAsync("/api/topic/v10/fact");

    // :todo add additional assertions
    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    mockObject.Verify(v => v.Handle(It.IsAny<CatFactQuery>(), It.IsAny<CancellationToken>()), Times.Once());
  }

  [Fact]
  public void ControllerRegistered()
  {
    // act
    var controller = NewServer().GetInstance<TopicController>();

    // :todo add additional assertions
    Assert.NotNull(controller);
  }

  [Fact]
  public async Task DefaultMethodNameAsync()
  {
    // prepare
    var mockrequestObject = new Mock<IRequestHandler<CreateTopicCommand, int>>();
    mockrequestObject.Setup(s => s.Handle(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(6));

    // action services => { }
    var host = NewServer((s) => s.Replace(mockrequestObject));
    var httpResult = await host.GetHttpClient().PostAsync("api/topic/v10", new CreateTopicCommand
    {
      Title = "string",
      Content = "string",
      AuthorId = 1
    }.ToHttpContent());

    // todo: assert
    Assert.Equal(HttpStatusCode.OK, httpResult.StatusCode);
    Assert.Equal("6", httpResult.Content.ReadAsStringAsync().Result);
  }

  [Fact]
  public async Task DefaultMethodName2Async()
  {
    // prepare
    var httpClient = new HttpRequestInterceptionBuilder().Requests().For(f => f.Method == HttpMethod.Get && f.RequestUri.AbsolutePath.EndsWith("/fact")).Responds().WithMediaType("application/json").WithContent(() => new CatFact { Fact = "Approximately 40,000 people are bitten by cats in the U.S. annually.", Length = 68 }.ToUtf8Bytes()).
            WithStatus(HttpStatusCode.OK).RegisterWith(HttpClientInterceptor);
    // action services => { }
    var host = NewServer();
    var httpResult = await host.GetHttpClient().GetAsync("/api/topic/v10/fact");
    // todo: assert
    Assert.Equal(HttpStatusCode.OK, httpResult.StatusCode);
    var n = httpResult.ReadJson<CatFact>();
    Assert.Equal(new CatFact { Fact = "Approximately 40,000 people are bitten by cats in the U.S. annually.", Length = 68 }, n);
  }

  [Fact]
  public async Task MediatorInputTestAsync()
  {
    // prepare
    var httpClient = new HttpRequestInterceptionBuilder().Requests().For(f => f.Method == HttpMethod.Get && f.RequestUri.AbsolutePath.EndsWith("/fact")).Responds().WithMediaType("application/json").WithContent(() => new CatFact { Fact = "Approximately 40,000 people are bitten by cats in the U.S. annually.", Length = 68 }.ToUtf8Bytes()).
            WithStatus(HttpStatusCode.OK).RegisterWith(HttpClientInterceptor);
    // action services => { }
    var host = NewServer();
    var mediator = host.GetInstance<IMediator>();
    var result = await mediator.Send(new CatFactQuery());
    // todo: assert
    Assert.Equal(new CatFact { Fact = "Approximately 40,000 people are bitten by cats in the U.S. annually.", Length = 68 }, result);
  }

  [Fact]
  public async Task DefaultMethodName4Async()
  {
    // prepare
    var httpClient = new HttpRequestInterceptionBuilder().Requests().For(f => f.Method == HttpMethod.Get && f.RequestUri.AbsolutePath.EndsWith("/fact"))
              .Responds().WithMediaType("application/json")
              .WithContent(() => new CatFact
              {
                Fact = "Cats take between 20-40 breaths per minute.",
                Length = 43
              }.ToUtf8Bytes())
              .WithStatus(HttpStatusCode.OK)
              .RegisterWith(HttpClientInterceptor);

    // action services => { }
    var host = NewServer();
    var mediator = host.GetInstance<IMediator>();
    var result = await mediator.Send(new CatFactQuery());

    // todo: assert
    Assert.Equal(new CatFact
    {
      Fact = "Cats take between 20-40 breaths per minute.",
      Length = 43
    }, result);
  }

  [Fact]
  public async Task DefaultMethodName5Async()
  {
    // action services => { }
    var host = NewServer();
    var dataSource = host.GetInstance<IDataSource>();
    var result = await dataSource.MultupleQueryAsync(1, "12");

    // todo: assert
    Assert.NotNull(result);
  }
}
