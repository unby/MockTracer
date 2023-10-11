using System.Net;
using MockTracer.Test.Api.Application.Features.HTTP;
using MockTracer.Test.Api.Domain;
using MediatR;
using Moq;
using Xunit.Abstractions;
using MockTracer.Test;

namespace DefaultNameSpace;

public class DefaultClassNameTest : SampleTestBase
{
    public DefaultClassNameTest(ITestOutputHelper output)
        : base(output)
    { }

    [Fact]
    public async Task DefaultMethodNameAsync()
    {
        // Arrange
        var requestMockObject = new Mock<IRequestHandler<CatFactQuery, CatFact>>();
        requestMockObject.Setup(s => s.Handle(It.IsAny<CatFactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CatFact
        {
            Fact = "Polydactyl cats (a cat with 1-2 extra toes on their paws) have this as a result of a genetic mutation. These cats are also referred to as \'Hemingway cats\' because writer Ernest Hemingway reportedly owned dozens of them at his home in Key West, Florida.",
            Length = 252
        });

        // Act
        var host = NewServer((s) => s.Replace(requestMockObject));
        var httpResult = await host.GetHttpClient().GetAsync("/api/topic/v10/fact");

        // todo: Assert
        Assert.Equal(HttpStatusCode.OK, httpResult.StatusCode);
        Assert.Equal(new CatFact
        {
            Fact = "Polydactyl cats (a cat with 1-2 extra toes on their paws) have this as a result of a genetic mutation. These cats are also referred to as \'Hemingway cats\' because writer Ernest Hemingway reportedly owned dozens of them at his home in Key West, Florida.",
            Length = 252
        }, httpResult.ReadJson<CatFact>());
    }
}