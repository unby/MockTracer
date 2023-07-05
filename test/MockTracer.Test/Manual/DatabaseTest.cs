using System.Net;
using Apps72.Dev.Data.DbMocker;
using MockTracer.Test.Api.Application.Features.SQL;
using Xunit.Abstractions;

namespace MockTracer.Test.Manual;
public class DatabaseTest : SampleTestBase
{
    public DatabaseTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task MultipleMockTestAsync()
    {
        var mockDB = new MockDbConnection();
        mockDB.Mocks.When(w => w.CommandText.Contains("SomeNumber"))
          .ReturnsDataset(
              MockTable.WithColumns("SomeNumber", "Name").AddRow(45, "sdf"),
              MockTable.WithColumns("SomeNumber", "Name").AddRow(45, "sdf").AddRow(76, "rtyrt"));
        // act
        var host = NewServer(services => services.SetTestDBConnectionProvider<IDbProvider>(mockDB));
        var result = await host.GetHttpClient().GetAsync("/api/topic/v10/sql-call?type=SingleRow");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(host.GetInstance<IDbProvider>().GetDbConnection(), mockDB);
    }
}
